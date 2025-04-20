using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Boss : EnemyState
{
	private EnemyBoss enemy;
	public IdleState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyBoss;
	}

	public override void Enter()
	{
		base.Enter();

		stateTimer = enemy.idleTime;
	}

	public override void Update()
	{
		base.Update();

		if (stateTimer < 0)
			stateMachine.ChangeState(enemy.moveState);
	}
}
