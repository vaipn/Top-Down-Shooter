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

	private float coverCheckTimer;
	public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();

		enemy.agent.isStopped = true;
		enemy.agent.velocity = Vector3.zero;

		bulletsPerShot = enemy.weaponData.bulletsToShoot;
		weaponCooldown = enemy.weaponData.GetWeaponCooldown();

		enemy.HoldWeapon();

		enemy.enemyVisuals.EnableIK(true, true);
	}

	public override void Exit()
	{
		base.Exit();
		enemy.enemyVisuals.EnableIK(false, false);
	}

	public override void Update()
	{
		base.Update();

		if (enemy.IsPlayerInAggressionRange() == false)
			stateMachine.ChangeState(enemy.advanceToPlayerState);


		ChangeCoverIfShould();

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

	private void ChangeCoverIfShould()
	{
		if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover)
			return;

		coverCheckTimer -= Time.deltaTime;

		if (coverCheckTimer < 0)
		{
			coverCheckTimer = 0.5f; // check if should change cover when player is in sight every 0.5 secs.

			if (IsPlayerInClearSight() || IsPlayerClose())
			{
				if (enemy.CanGetCover())
					stateMachine.ChangeState(enemy.runToCoverState);
			}
		}
	}

	#region Cover system region

	private bool IsPlayerClose()
	{
		return Vector3.Distance(enemy.transform.position, enemy.playerTransform.position) < enemy.safeDistance;
	}

	private bool IsPlayerInClearSight()
	{
		Vector3 rayLevel = enemy.transform.position + Vector3.up * 0.4f;
		Vector3 directionToPlayer = enemy.playerTransform.position - rayLevel;
		float distanceToPlayer = directionToPlayer.magnitude;

		if (Physics.Raycast(rayLevel, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer))
		{
			Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
			return hit.collider.gameObject.GetComponentInParent<Player>();
		}

		return false;
	}

	#endregion

	#region Weapon region
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
	#endregion
}
