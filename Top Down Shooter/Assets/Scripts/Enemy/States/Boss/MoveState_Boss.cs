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
		enemy.agent.isStopped = false;

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

		if (enemy.inBattleMode)
		{
			Vector3 playerPos = enemy.playerTransform.position;

			enemy.agent.SetDestination(playerPos);

			if (enemy.CanDoJumpAttack())
				stateMachine.ChangeState(enemy.jumpAttackState);
			else if (enemy.PlayerInAttackRange())
				stateMachine.ChangeState(enemy.attackState);
		}
		else
		{
			if (Vector3.Distance(enemy.transform.position, destination) < 0.25f)
				stateMachine.ChangeState(enemy.idleState);
		}

		
	}
}
