using UnityEngine;

public class EnemyRange : Enemy
{
	public EnemyRange_WeaponType weaponType;


	public Transform weaponHolder;

	public float fireRate = 1; // Bullets per second
	public GameObject bulletPrefab;
	public Transform gunPoint;
	public float bulletSpeed = 20;
	public int bulletsToShoot = 5; // bullets to shoot before weapon goes on cooldown
	public float weaponCooldown = 1.5f; // weapon cooldown after all bullets are shot

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
		rbNewBullet.mass = 20 / bulletSpeed;
		rbNewBullet.velocity = bulletsDirection * bulletSpeed;
	}
}
