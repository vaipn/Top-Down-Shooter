using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState_Range : EnemyState
{
	private EnemyRange enemy;

	private float lastTimeShot = -10;
	private int bulletsShot = 0;
	public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();

		enemy.FaceTarget(enemy.playerTransform.position);

		if (WeaponOutOfBullets())
		{
			if (WeaponCooleddown())
			{
				bulletsShot = 0;
			}
			return;
		}

		if (CanShoot())
		{
			Shoot();
		}
	}

	private bool WeaponCooleddown() => Time.time > lastTimeShot + enemy.weaponCooldown;

	private bool WeaponOutOfBullets() => bulletsShot >= enemy.bulletsToShoot;

	private bool CanShoot() => Time.time > lastTimeShot + 1 / enemy.fireRate;

	private void Shoot()
	{
		enemy.FireSingleBullet();
		lastTimeShot = Time.time;
		bulletsShot++;
	}
}
