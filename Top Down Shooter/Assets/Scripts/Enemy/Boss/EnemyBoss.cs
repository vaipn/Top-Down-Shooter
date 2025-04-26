using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemyBoss : Enemy
{
	[Header("Boss details")]
	public float actionCooldown = 10;
	public float attackRange;

	[Header("Flame Throw Ability")]
	public ParticleSystem flameThrower;
	public float flameThrowDuration;
	public float abilityCooldown;
	private float lastTimeUsedAbility;
	public bool flameThrowActive {  get; private set; }

	[Header("Jump attack")]
	public float jumpAttackCooldown = 5;
	private float lastTimeJumped;
	public float travelTimeToTarget = 1;
	public float minJumpDistanceRequired;
	public float impactRadius = 2.5f;
	public float impactPower = 5;
	[SerializeField] private float upforceModifier = 10;
	[Space]
	[SerializeField] private LayerMask whatToIgnore;
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
		abilityState = new AbilityState_Boss(this, stateMachine, "FlameAbility");
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
	}

	public override void GetHit()
	{
		base.GetHit();

		if (healthPoint <= 0 && stateMachine.currentState != deadState)
			stateMachine.ChangeState(deadState);
	}

	public override void EnterBattleMode()
	{
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

	public bool CanDoAbility()
	{
		if (Time.time > lastTimeUsedAbility + abilityCooldown)
		{
			return true;
		}

		return false;
	}

	public void SetAbilityOnCooldown() => lastTimeUsedAbility = Time.time;

	public void JumpImpact()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

		foreach (Collider collider in colliders)
		{
			Rigidbody rb = collider.GetComponent<Rigidbody>();

			if (rb != null)
				rb.AddExplosionForce(impactPower, transform.position, impactRadius, upforceModifier, ForceMode.Impulse);
		}
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
		Vector3 enemyEyeLevel = transform.position + new Vector3(0, 1.55f, 0);
		Vector3 playerEyeLevel = playerTransform.position + new Vector3(0, 1.55f, 0);
		Vector3 directionToPlayer = (playerEyeLevel - enemyEyeLevel).normalized;

		if (Physics.Raycast(enemyEyeLevel, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
		{
			if (hit.transform == playerTransform || hit.transform.parent == playerTransform)
				return true;
		}
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
	}
}
