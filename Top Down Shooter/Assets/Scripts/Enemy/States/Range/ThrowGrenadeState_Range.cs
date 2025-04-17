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

		enemy.DisableHeldWeapon();
		enemy.enemyVisuals.EnableIK(false, false);
		enemy.enemyVisuals.EnableSecondaryWeaponModel(true);
	}

	public override void Update()
	{
		base.Update();

		Vector3 playerPos = enemy.playerTransform.position + Vector3.up;
		enemy.FaceTarget(playerPos);
		enemy.aim.position = playerPos;

		if (triggerCalled)
			stateMachine.ChangeState(enemy.battleState);
	}

	public override void AbilityTrigger()
	{
		base.AbilityTrigger();

		enemy.ThrowGrenade();
	}
	public override void Exit()
	{
		base.Exit();
		enemy.enemyVisuals.EnableSecondaryWeaponModel(false);
	}
}
