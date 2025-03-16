using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
	private EnemyMelee enemy;
	public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
	}

	public override void Enter()
	{
		base.Enter();

		if (!enemy.PlayerInAttackRange())
			enemy.SheathWeapon();

		enemy.agent.isStopped = true; //meant to fix bug where enemy still glides to its destination after entering this state

		Debug.Log("I enter Recovery state");
	}

	public override void Exit()
	{
		base.Exit();

		Debug.Log("I exit Recovery state");
	}

	public override void Update()
	{
		base.Update();

		enemy.transform.rotation = enemy.FaceTarget(enemy.playerTransform.position);

		if (triggerCalled)
		{
			if (enemy.PlayerInAttackRange())
				stateMachine.ChangeState(enemy.attackState);
			else
				stateMachine.ChangeState(enemy.chaseState);
		}

			
	}
}
