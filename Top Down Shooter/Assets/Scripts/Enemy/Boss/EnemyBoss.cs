using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy
{
	public float attackRange;
	public IdleState_Boss idleState {  get; private set; }
	public MoveState_Boss moveState { get; private set; }
	public AttackState_Boss attackState { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		idleState = new IdleState_Boss(this, stateMachine, "Idle");
		moveState = new MoveState_Boss(this, stateMachine, "Move");
		attackState = new AttackState_Boss(this, stateMachine, "Attack");
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

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();

		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
