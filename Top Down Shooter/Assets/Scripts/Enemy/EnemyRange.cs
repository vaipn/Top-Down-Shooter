using System.Collections.Generic;
using UnityEngine;

public enum CoverPerk { Unavailable, CanTakeCover, CanTakeAndChangeCover}
public enum UnstoppablePerk { Unavailable, Unstoppable}
public enum GrenadePerk { Unavailable, CanThrowGrenade}
public class EnemyRange : Enemy
{
	[Header("Enemy perks")]
	public CoverPerk coverPerk;
	public UnstoppablePerk unstoppablePerk;
	public GrenadePerk grenadePerk;

	[Header("Grenade perk")]
	public GameObject grenadePrefab;
	public int grenadeDamage;
	public float timeToReachTarget = 1.2f;
	public float timeToExplode = 0.75f;
	public float impactPower;
	public float grenadeCooldown;
	private float lastTimeGrenadeThrown = -10;
	[SerializeField] private Transform grenadeStartPoint;

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
	public float attackDelay;
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
	public ThrowGrenadeState_Range throwGrenadeState { get; private set; }
	public DeadState_Range deadState { get; private set; }
	#endregion

	protected override void Awake()
	{
		base.Awake();

		idleState = new IdleState_Range(this, stateMachine, "Idle");
		moveState = new MoveState_Range(this, stateMachine, "Move");
		battleState = new BattleState_Range(this, stateMachine, "Battle");
		runToCoverState = new RunToCoverState_Range(this, stateMachine, "Cover");
		advanceToPlayerState = new AdvanceToPlayerState_Range(this, stateMachine, "Advance");
		throwGrenadeState = new ThrowGrenadeState_Range(this, stateMachine, "ThrowGrenade");
		deadState = new DeadState_Range(this, stateMachine, "Idle"); //idle is placeholder, we using ragdoll.
	}

	protected override void Start()
	{
		base.Start();

		playersBody = playerTransform.GetComponent<Player>().playerBody;
		aim.parent = null;

		InitializePerk();

		enemyVisuals.SetupLook();
		stateMachine.Initialize(idleState);
		
		SetupWeapon();
	}

	protected override void Update()
	{
		base.Update();

		stateMachine.currentState.Update();
	}

	public override void Die()
	{
		base.Die();

		if (stateMachine.currentState != deadState)
			stateMachine.ChangeState(deadState);
	}
	public bool CanThrowGrenade()
	{
		if (grenadePerk == GrenadePerk.Unavailable)
			return false;

		if (Vector3.Distance(playerTransform.position, transform.position) < safeDistance)
			return false; // player too close, don't throw grenade

		if (Time.time > lastTimeGrenadeThrown + grenadeCooldown)
			return true;

		return false;
	}

	public void ThrowGrenade()
	{
		lastTimeGrenadeThrown = Time.time;

		enemyVisuals.EnableGrenadeModel(false);

		GameObject newGrenade = ObjectPool.instance.GetObjectFromPool(grenadePrefab, grenadeStartPoint);

		EnemyGrenade grenadeScript = newGrenade.GetComponent<EnemyGrenade>();

		if (stateMachine.currentState == deadState)
		{
			grenadeScript.SetupGrenade(whatIsAlly, transform.position, 1, timeToExplode, impactPower, grenadeDamage);
			return;
		}

		grenadeScript.SetupGrenade(whatIsAlly, playerTransform.position, timeToReachTarget, timeToExplode, impactPower, grenadeDamage);
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

		GameObject newBullet = ObjectPool.instance.GetObjectFromPool(bulletPrefab, gunPoint);
		newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

		newBullet.GetComponent<Bullet>().BulletSetup(whatIsAlly, weaponData.bulletDamage);

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
