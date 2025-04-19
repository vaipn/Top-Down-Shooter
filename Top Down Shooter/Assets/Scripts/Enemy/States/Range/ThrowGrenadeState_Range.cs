using UnityEngine;

public class ThrowGrenadeState_Range : EnemyState
{
	private EnemyRange enemy;

	public bool finishedThrowingGrenade { get; private set; }
	public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();

		finishedThrowingGrenade = false;

		enemy.DisableHeldWeapon();
		enemy.enemyVisuals.EnableIK(false, false);
		enemy.enemyVisuals.EnableSecondaryWeaponModel(true);
		enemy.enemyVisuals.EnableGrenadeModel(true);
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
		finishedThrowingGrenade = true;
		enemy.ThrowGrenade();
	}
	public override void Exit()
	{
		base.Exit();
		enemy.enemyVisuals.EnableSecondaryWeaponModel(false);
	}
}
