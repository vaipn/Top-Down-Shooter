using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState_Range : EnemyState
{
	private EnemyRange enemy;

	private float lastTimeShot = -10;
	private int bulletsShot = 0;

	private int bulletsPerShot;
	private float weaponCooldown;
	public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();

		bulletsPerShot = enemy.weaponData.bulletsToShoot;
		weaponCooldown = enemy.weaponData.GetWeaponCooldown();

		enemy.HoldWeapon();

		enemy.enemyVisuals.EnableIK(true);
	}

	public override void Exit()
	{
		base.Exit();
		enemy.enemyVisuals.EnableIK(false);
	}

	public override void Update()
	{
		base.Update();

		enemy.FaceTarget(enemy.playerTransform.position);

		if (WeaponOutOfBullets())
		{
			if (WeaponCooleddown())
			{
				ResetWeapon();
			}
			return;
		}

		if (CanShoot())
		{
			Shoot();
		}
	}

	private void ResetWeapon()
	{
		bulletsShot = 0;
		bulletsPerShot = enemy.weaponData.bulletsToShoot;
		weaponCooldown = enemy.weaponData.GetWeaponCooldown();
	}

	private bool WeaponCooleddown() => Time.time > lastTimeShot + weaponCooldown;

	private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerShot;

	private bool CanShoot() => Time.time > lastTimeShot + 1 / enemy.weaponData.fireRate;

	private void Shoot()
	{
		enemy.FireSingleBullet();
		lastTimeShot = Time.time;
		bulletsShot++;
	}
}
