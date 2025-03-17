using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Melee : EnemyState
{
	private EnemyMelee enemy;
	public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
	}

	public override void Enter()
	{
		base.Enter();

		enemy.anim.enabled = false;
		enemy.agent.isStopped = true;
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();
	}
}
