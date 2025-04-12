using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : Enemy
{
	[Header("Cover system")]
	public bool canUseCovers = true;
	public CoverPoint lastCover;
	public List<Cover> allCovers = new List<Cover>();


	[Header("Weapon details")]
	public EnemyRange_WeaponType weaponType;
	public EnemyRange_WeaponData weaponData;
	[Space]

	public Transform weaponHolder;
	public Transform gunPoint;
	public GameObject bulletPrefab;

	[SerializeField] List<EnemyRange_WeaponData> availableWeaponData;

	public IdleState_Range idleState { get; private set; }
	public MoveState_Range moveState { get; private set; }
	public BattleState_Range battleState { get; private set; }
	public RunToCoverState_Range runToCoverState { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		idleState = new IdleState_Range(this, stateMachine, "Idle");
		moveState = new MoveState_Range(this, stateMachine, "Move");
		battleState = new BattleState_Range(this, stateMachine, "Battle");
		runToCoverState = new RunToCoverState_Range(this, stateMachine, "Cover");
	}

	protected override void Start()
	{
		base.Start();

		stateMachine.Initialize(idleState);
		enemyVisuals.SetupLook();
		SetupWeapon();

		allCovers.AddRange(CollectNearbyCovers());
	}

	protected override void Update()
	{
		base.Update();

		stateMachine.currentState.Update();
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

	public Transform AttemptToFindCover()
	{
		List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();

		foreach (Cover cover in allCovers)
		{
			collectedCoverPoints.AddRange(cover.GetValidCoverPoints());
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
			lastCover = closestCoverPoint;
			lastCover.SetOccupied(true); // newly assigned cover
		}

		return lastCover.transform;
	}
	#endregion

	public override void EnterBattleMode()
	{
		if (inBattleMode)
			return;

		base.EnterBattleMode();

		if (canUseCovers)
			stateMachine.ChangeState(runToCoverState);
		else
			stateMachine.ChangeState(battleState);
	}

	public void FireSingleBullet()
	{
		anim.SetTrigger("Shoot");

		Vector3 bulletsDirection = ((playerTransform.position + Vector3.up) - gunPoint.position).normalized;

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
}
