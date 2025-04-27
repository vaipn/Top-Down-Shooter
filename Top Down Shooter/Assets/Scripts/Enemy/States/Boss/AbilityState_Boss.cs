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

		if (ShouldDisableFlameThrower())
			DisableFlamethrower();

		if (triggerCalled)
			stateMachine.ChangeState(enemy.moveState);
	}


	public override void Exit()
	{
		base.Exit();
		enemy.SetAbilityOnCooldown();
		enemy.bossVisuals.ResetBatteries();
		enemy.bossVisuals.EnableWeaponTrails(false);
	}

	private bool ShouldDisableFlameThrower() => stateTimer < 0;
	public void DisableFlamethrower()
	{
		if (enemy.bossWeaponType != BossWeaponType.FlameThrower)
			return;

		if (enemy.flameThrowActive == false)
			return;

		enemy.ActivateFlameThrower(false);
	}

	public override void AbilityTrigger()
	{
		base.AbilityTrigger();

		if (enemy.bossWeaponType == BossWeaponType.FlameThrower)
		{
			enemy.ActivateFlameThrower(true);
			enemy.bossVisuals.DischargeBatteries();
		}

		if (enemy.bossWeaponType == BossWeaponType.Hammer)
		{
			enemy.ActivateHammer();
		}
		
	}
}
