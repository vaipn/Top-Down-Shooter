using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum BossWeaponType { FlameThrower, Hammer}

public class EnemyBoss : Enemy
{
	[Header("Boss details")]
	public BossWeaponType bossWeaponType;
	public float actionCooldown = 10;
	public float attackRange;

	[Header("Ability")]
	public float abilityCooldown;
	private float lastTimeUsedAbility;
	public float minAbilityDistance;

	[Header("Flame Throw")]
	public ParticleSystem flameThrower;
	public float flameThrowDuration;
	public float flameDamageCooldown;
	public bool flameThrowActive {  get; private set; }

	[Header("Hammer")]
	public GameObject hammerFxPrefab;

	[Header("Jump attack")]
	public float jumpAttackCooldown = 5;
	private float lastTimeJumped;
	public float travelTimeToTarget = 1;
	public float minJumpDistanceRequired;
	public float impactRadius = 2.5f;
	public float impactPower = 5;
	public Transform impactPoint;
	[SerializeField] private float upforceModifier = 10;
	[Space]
	[SerializeField] private LayerMask whatToIgnore;

	[Header("Simple attack")]
	[SerializeField] private Transform[] damagePoints;
	[SerializeField] private float damageRadius;
	[SerializeField] private GameObject meleeAttackFx;
	public IdleState_Boss idleState {  get; private set; }
	public MoveState_Boss moveState { get; private set; }
	public AttackState_Boss attackState { get; private set; }
	public JumpAttackState_Boss jumpAttackState { get; private set; }
	public AbilityState_Boss abilityState { get; private set; }
	public DeadState_Boss deadState { get; private set; }

	public EnemyBossVisuals bossVisuals { get; private set; }
	protected override void Awake()
	{
		base.Awake();

		bossVisuals = GetComponent<EnemyBossVisuals>();

		idleState = new IdleState_Boss(this, stateMachine, "Idle");
		moveState = new MoveState_Boss(this, stateMachine, "Move");
		attackState = new AttackState_Boss(this, stateMachine, "Attack");
		jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
		abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
		deadState = new DeadState_Boss(this, stateMachine, "Idle"); //idle is just a placeholder, ragdoll is used.
	}

	protected override void Start()
	{
		base.Start();

		stateMachine.Initialize(idleState);
	}

	protected override void Update()
	{
		base.Update();

		stateMachine.currentState.Update();

		if (ShouldEnterBattleMode())
			EnterBattleMode();

		MeleeAttackCheck(damagePoints, damageRadius, meleeAttackFx);
	}

	public override void Die()
	{
		base.Die();

		if (stateMachine.currentState != deadState)
			stateMachine.ChangeState(deadState);
	}

	public override void EnterBattleMode()
	{
		if (inBattleMode)
			return;

		base.EnterBattleMode();
		stateMachine.ChangeState(moveState);
	}

	public bool PlayerInAttackRange() => Vector3.Distance(transform.position, playerTransform.position) < attackRange;

	public void ActivateFlameThrower(bool activate)
	{
		flameThrowActive = activate;

		if (!activate)
		{
			anim.SetTrigger("StopFlameThrower");
			flameThrower.Stop();
			Debug.Log("Flame stopped");
			return;
		}
		Debug.Log("Flame activated");

		var mainModule = flameThrower.main;
		var childModule = flameThrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;

		mainModule.duration = flameThrowDuration;
		childModule.duration = flameThrowDuration;

		flameThrower.Clear();
		flameThrower.Play();
	}

	public void ActivateHammer()
	{
		GameObject newHammerFx = ObjectPool.instance.GetObjectFromPool(hammerFxPrefab, impactPoint);

		ObjectPool.instance.ReturnObjectToPoolWithDelay(newHammerFx, 1);
	}

	public bool CanDoAbility()
	{
		bool playerWithinDistance = Vector3.Distance(transform.position, playerTransform.position) < minAbilityDistance;

		if (!playerWithinDistance)
			return false;

		if (Time.time > lastTimeUsedAbility + abilityCooldown)
		{
			return true;
		}

		return false;
	}

	public void SetAbilityOnCooldown() => lastTimeUsedAbility = Time.time;

	public void JumpImpact()
	{
		Transform impactPoint = this.impactPoint;

		if (impactPoint == null)
			impactPoint = transform;

		MassDamage(impactPoint.position, impactRadius);
	}

	private void MassDamage(Vector3 impactPoint, float impactRadius)
	{
		HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
		Collider[] colliders = Physics.OverlapSphere(impactPoint, impactRadius, ~whatIsAlly);

		foreach (Collider collider in colliders)
		{
			IDamagable damagable = collider.GetComponent<IDamagable>();

			if (damagable != null)
			{
				GameObject rootEntity = collider.transform.root.gameObject;

				if (uniqueEntities.Add(rootEntity) == false)
					continue;

				damagable.TakeDamage();
			}

			ApplyPhysicalForceTo(impactPoint, impactRadius, collider);
		}
	}

	private void ApplyPhysicalForceTo(Vector3 impactPoint, float impactRadius, Collider collider)
	{
		Rigidbody rb = collider.GetComponent<Rigidbody>();

		if (rb != null)
			rb.AddExplosionForce(impactPower, impactPoint, impactRadius, upforceModifier, ForceMode.Impulse);
	}

	public bool CanDoJumpAttack()
	{
		float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

		if (distanceToPlayer < minJumpDistanceRequired)
			return false;

		if (Time.time > lastTimeJumped + jumpAttackCooldown && IsPlayerInClearSight())
		{
			return true;
		}
		return false;
	}

	public void SetJumpAttackOnCooldown() => lastTimeJumped = Time.time;

	public bool IsPlayerInClearSight()
	{
		Vector3 enemyEyeLevel = transform.position + new Vector3(0, 1.5f, 0);
		Vector3 playerLevel = playerTransform.position + new Vector3(0, 1.4f, 0);
		Vector3 directionToPlayer = (playerLevel - enemyEyeLevel).normalized;

		if (Physics.Raycast(enemyEyeLevel, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
		{
			if (hit.transform == playerTransform || hit.transform.parent == playerTransform)
			{
				Debug.Log("Player is in clear sight");
				return true;
			}
		}
		Debug.Log("Player is not in clear sight");
		return false;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();

		Gizmos.DrawWireSphere(transform.position, attackRange);

		if (playerTransform != null)
		{
			Vector3 enemyEyeLevel = transform.position + new Vector3(0, 1.55f, 0);
			Vector3 playerEyeLevel = playerTransform.position + new Vector3(0, 1.55f, 0);

			Gizmos.color = Color.green;

			Gizmos.DrawLine(enemyEyeLevel, playerEyeLevel);
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

		if (damagePoints.Length > 0)
		{
			foreach (var damagePoint in damagePoints)
			{
				Gizmos.DrawWireSphere(damagePoint.position, damageRadius);
			}
		}
	}
}
