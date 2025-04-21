using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy
{
	public float attackRange;

	[Header("Jump attack")]
	public float jumpAttackCooldown = 5;
	private float lastTimeJumped;
	public float travelTimeToTarget = 1;
	public float minJumpDistanceRequired;
	[Space]
	[SerializeField] private LayerMask whatToIgnore;
	public IdleState_Boss idleState {  get; private set; }
	public MoveState_Boss moveState { get; private set; }
	public AttackState_Boss attackState { get; private set; }
	public JumpAttackState_Boss jumpAttackState { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		idleState = new IdleState_Boss(this, stateMachine, "Idle");
		moveState = new MoveState_Boss(this, stateMachine, "Move");
		attackState = new AttackState_Boss(this, stateMachine, "Attack");
		jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
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

	public override void EnterBattleMode()
	{
		base.EnterBattleMode();
		stateMachine.ChangeState(moveState);
	}

	public bool PlayerInAttackRange() => Vector3.Distance(transform.position, playerTransform.position) < attackRange;

	public bool CanDoJumpAttack()
	{
		float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

		if (distanceToPlayer < minJumpDistanceRequired)
			return false;

		if (Time.time > lastTimeJumped + jumpAttackCooldown && IsPlayerInClearSight())
		{
			lastTimeJumped = Time.time;
			return true;
		}
		return false;
	}

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
