using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceToPlayerState_Range : EnemyState
{
	private EnemyRange enemy;
	private Vector3 playerPos;
	public AdvanceToPlayerState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();

		enemy.agent.isStopped = false;
		enemy.agent.speed = enemy.advanceSpeed;
	}

	public override void Update()
	{
		base.Update();

		playerPos = enemy.playerTransform.position;

		enemy.agent.SetDestination(playerPos);
		enemy.FaceTarget(enemy.agent.steeringTarget);

		if (Vector3.Distance(enemy.transform.position, playerPos) < enemy.advanceStoppingDistance)
			stateMachine.ChangeState(enemy.battleState);
	}
}
