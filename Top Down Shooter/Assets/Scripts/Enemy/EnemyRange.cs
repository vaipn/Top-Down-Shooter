using System.Collections.Generic;
using UnityEngine;

public enum CoverPerk { Unavailable, CanTakeCover, CanTakeAndChangeCover}
public enum UnstoppablePerk { Unavailable, Unstoppable}

public class EnemyRange : Enemy
{
	[Header("Enemy perks")]
	public CoverPerk coverPerk;
	public UnstoppablePerk unstoppablePerk;

	[Header("Advance perk")]
	public float advanceSpeed;
	public float advanceStoppingDistance;
	public float advanceDuration = 2.5f;

	[Header("Cover system")]
	public float minCoverTime;
	public float safeDistance;
	public CoverPoint lastCover { get; private set; }
	public CoverPoint currentCover { get; private set; }


	[Header("Weapon details")]
	public EnemyRange_WeaponType weaponType;
	public EnemyRange_WeaponData weaponData;

	[Space]
	public Transform weaponHolder;
	public Transform gunPoint;
	public GameObject bulletPrefab;

	[SerializeField] List<EnemyRange_WeaponData> availableWeaponData;

	[Header("Aim details")]
	public float slowAim = 4;
	public float fastAim = 20;
	public Transform aim;
	public Transform playersBody;
	public LayerMask whatToIgnore;

	#region States
	public IdleState_Range idleState { get; private set; }
	public MoveState_Range moveState { get; private set; }
	public BattleState_Range battleState { get; private set; }
	public RunToCoverState_Range runToCoverState { get; private set; }
	public AdvanceToPlayerState_Range advanceToPlayerState { get; private set; }
	#endregion

	protected override void Awake()
	{
		base.Awake();

		idleState = new IdleState_Range(this, stateMachine, "Idle");
		moveState = new MoveState_Range(this, stateMachine, "Move");
		battleState = new BattleState_Range(this, stateMachine, "Battle");
		runToCoverState = new RunToCoverState_Range(this, stateMachine, "Cover");
		advanceToPlayerState = new AdvanceToPlayerState_Range(this, stateMachine, "Advance");
	}

	protected override void Start()
	{
		base.Start();

		playersBody = playerTransform.GetComponent<Player>().playerBody;
		aim.parent = null;

		InitializePerk();

		stateMachine.Initialize(idleState);
		enemyVisuals.SetupLook();
		SetupWeapon();
	}

	protected override void Update()
	{
		base.Update();

		stateMachine.currentState.Update();
	}

	protected override void InitializePerk()
	{
		if (IsUnstoppable())
		{
			advanceSpeed = 1;
			anim.SetFloat("AdvanceAnimIndex", 1);
		}
	}

	#region Cover System
	private List<Cover> CollectNearbyCovers()
	{
		float coverDistanceToCheck = 30;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverDistanceToCheck);
		List<Cover> collectedCovers = new List<Cover>();

		foreach (Collider collider in hitColliders)
		{
			Cover cover = collider.GetComponent<Cover>();

			if (cover != null && collectedCovers.Contains(cover) == false)
				collectedCovers.Add(cover);
		}

		return collectedCovers;
	}

	public bool CanGetCover()
	{
		if (coverPerk == CoverPerk.Unavailable)
			return false;

		currentCover = AttemptToFindCover()?.GetComponent<CoverPoint>();

		if (lastCover != currentCover && currentCover != null)
			return true;

		Debug.LogWarning("No cover found!");
		return false;
	}

	private Transform AttemptToFindCover()
	{
		List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();

		foreach (Cover cover in CollectNearbyCovers())
		{
			collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
		}

		CoverPoint closestCoverPoint = null;
		float closestDistance = float.MaxValue;

		foreach (CoverPoint coverPoint in collectedCoverPoints)
		{
			float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);
			if (currentDistance < closestDistance)
			{
				closestCoverPoint = coverPoint;
				closestDistance = currentDistance;
			}
		}

		if (closestCoverPoint != null)
		{
			lastCover?.SetOccupied(false); // previous cover
			lastCover = currentCover;

			currentCover = closestCoverPoint;
			currentCover.SetOccupied(true); // newly assigned cover

			return currentCover.transform;
		}

		return null;
	}
	#endregion

	public override void EnterBattleMode()
	{
		if (inBattleMode)
			return;

		base.EnterBattleMode();

		if (CanGetCover())
			stateMachine.ChangeState(runToCoverState);
		else
			stateMachine.ChangeState(battleState);
	}

	public void FireSingleBullet()
	{
		anim.SetTrigger("Shoot");

		Vector3 bulletsDirection = (aim.position - gunPoint.position).normalized;

		GameObject newBullet = ObjectPool.instance.GetObjectFromPool(bulletPrefab);
		newBullet.transform.position = gunPoint.position;
		newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

		newBullet.GetComponent<EnemyBullet>().BulletSetup();

		Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

		Vector3 bulletDirectionWithSpread = weaponData.ApplySpread(bulletsDirection);

		rbNewBullet.mass = 20 / weaponData.bulletSpeed;
		rbNewBullet.velocity = bulletDirectionWithSpread * weaponData.bulletSpeed;
	}

	private void SetupWeapon()
	{
		List<EnemyRange_WeaponData> filteredData = new List<EnemyRange_WeaponData>();

		foreach (var weaponData in availableWeaponData)
		{
			if (weaponData.weaponType == weaponType)
				filteredData.Add(weaponData);
		}

		if (filteredData.Count > 0)
		{
			int randomIndex = Random.Range(0, filteredData.Count);
			weaponData = filteredData[randomIndex];
		}
		else
			Debug.LogWarning("No available weapon data was found!");

		gunPoint = enemyVisuals.currentHeldWeaponModel.GetComponent<EnemyRangeWeaponModel>().gunPoint;
	}

	#region Enemy's aiming system
	public void UpdateAimPosition()
	{
		float aimSpeed = IsAimOnPlayer() ? fastAim : slowAim;
		aim.position = Vector3.MoveTowards(aim.position, playersBody.position, aimSpeed * Time.deltaTime);
	}

	public bool IsAimOnPlayer()
	{
		float distanceAimToPlayer = Vector3.Distance(aim.position, playerTransform.position);

		return distanceAimToPlayer < 2;
	}

	public bool IsSeeingPlayer()
	{
		Vector3 enemyPosition = transform.position + Vector3.up;
		Vector3 directionToPlayer = playersBody.position - enemyPosition;

		if (Physics.Raycast(enemyPosition, directionToPlayer.normalized, out RaycastHit hit, Mathf.Infinity, ~whatToIgnore))
		{
			if (hit.transform.root == playerTransform.root)
			{
				UpdateAimPosition();
				return true;
			}
		}

		return false;
	}
	#endregion

	public bool IsUnstoppable() => unstoppablePerk == UnstoppablePerk.Unstoppable;
}
