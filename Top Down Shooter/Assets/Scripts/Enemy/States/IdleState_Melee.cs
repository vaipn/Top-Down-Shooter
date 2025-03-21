
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Melee : EnemyState
{
	private EnemyMelee enemy;
	public IdleState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
	}

	public override void Enter()
	{
		base.Enter();

		stateTimer = enemyBase.idleTime;

		Debug.Log("I enter Idle state");
	}

	public override void Exit()
	{
		base.Exit();

		Debug.Log("I exit idle state");
	}

	public override void Update()
	{
		base.Update();

		if (stateTimer < 0)
			stateMachine.ChangeState(enemy.moveState);
	}
}
