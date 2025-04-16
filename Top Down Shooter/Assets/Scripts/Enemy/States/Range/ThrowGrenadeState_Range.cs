using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenadeState_Range : EnemyState
{
	private EnemyRange enemy;
	public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();
	}

	public override void Update()
	{
		base.Update();

		enemy.FaceTarget(enemy.playerTransform.position);

		if (triggerCalled)
			stateMachine.ChangeState(enemy.battleState);
	}

	public override void AbilityTrigger()
	{
		base.AbilityTrigger();

		enemy.ThrowGrenade();
	}
}
