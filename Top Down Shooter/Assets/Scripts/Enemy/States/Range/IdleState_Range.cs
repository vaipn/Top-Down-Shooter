using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Range : EnemyState
{
    private EnemyRange enemy;

	public IdleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();
		enemy.HoldWeapon();
		enemy.anim.SetFloat("IdleAnimIndex", Random.Range(0, 3));

		//enemy.enemyVisuals.EnableIK(true, false);
		stateTimer = enemy.idleTime;
	}

	public override void Update()
	{
		base.Update();

		if (stateTimer < 0)
			stateMachine.ChangeState(enemy.moveState);
	}
}
