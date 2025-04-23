using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Boss : EnemyState
{
	private EnemyBoss enemy;
	public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyBoss;
	}

	public override void Enter()
	{
		base.Enter();

		stateTimer = enemy.flameThrowDuration;

		enemy.agent.isStopped = true;
		enemy.agent.velocity = Vector3.zero;
	}

	public override void Update()
	{
		base.Update();

		enemy.FaceTarget(enemy.playerTransform.position);

		if (stateTimer < 0 && enemy.flameThrowActive)
			enemy.ActivateFlameThrower(false);

		if (triggerCalled)
			stateMachine.ChangeState(enemy.moveState);
	}
	public override void Exit()
	{
		base.Exit();
		enemy.SetAbilityOnCooldown();
	}
	public override void AbilityTrigger()
	{
		base.AbilityTrigger();
		enemy.ActivateFlameThrower(true);
	}
}
