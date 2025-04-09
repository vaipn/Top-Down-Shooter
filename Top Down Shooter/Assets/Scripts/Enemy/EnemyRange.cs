using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : Enemy
{
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

	protected override void Awake()
	{
		base.Awake();

		idleState = new IdleState_Range(this, stateMachine, "Idle");
		moveState = new MoveState_Range(this, stateMachine, "Move");
		battleState = new BattleState_Range(this, stateMachine, "Battle");
	}

	protected override void Start()
	{
		base.Start();

		stateMachine.Initialize(idleState);
		enemyVisuals.SetupLook();
		SetupWeapon();
	}

	protected override void Update()
	{
		base.Update();

		stateMachine.currentState.Update();
	}

	public override void EnterBattleMode()
	{
		if (inBattleMode)
			return;

		base.EnterBattleMode();

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
