using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_Boss : EnemyState
{
	private EnemyBoss enemy;

	private Vector3 destination;
	public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyBoss;
	}

	public override void Enter()
	{
		base.Enter();

		enemy.agent.speed = enemy.walkSpeed;

		destination = enemy.GetPatrolDestination();

		enemy.agent.SetDestination(destination);
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();

		enemy.FaceTarget(enemy.agent.steeringTarget);

		if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.05f)
			stateMachine.ChangeState(enemy.idleState);
	}
}
