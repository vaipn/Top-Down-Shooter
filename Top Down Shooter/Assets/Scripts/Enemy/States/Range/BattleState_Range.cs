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

	private bool firstTimeAttack = true;
	public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();
		SetupValuesForFirstAttack();

		enemy.agent.isStopped = true;
		enemy.agent.velocity = Vector3.zero;

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

		if (enemy.IsSeeingPlayer())
			enemy.FaceTarget(enemy.aim.position);

		if (MustAdvancePlayer())
			stateMachine.ChangeState(enemy.advanceToPlayerState);

		ChangeCoverIfShould();


		if (WeaponOutOfBullets())
		{
			if (enemy.IsUnstoppable() && UnstoppableWalkReady())
			{
				enemy.advanceDuration = weaponCooldown;
				stateMachine.ChangeState(enemy.advanceToPlayerState);
			}

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

	private bool MustAdvancePlayer()
	{
		if (enemy.IsUnstoppable())
			return false;

		return enemy.IsPlayerInAggressionRange() == false && ReadyToLeaveCover();
	}

	private bool UnstoppableWalkReady()
	{
		float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.playerTransform.position);
		bool outOfStoppingDistance = distanceToPlayer > enemy.advanceStoppingDistance;
		bool unstoppableWalkOnCooldown = Time.time < enemy.weaponData.minWeaponCooldown + enemy.advanceToPlayerState.lastTimeAdvanced;

		return outOfStoppingDistance && !unstoppableWalkOnCooldown;
	}

	#region Cover system region

	private bool ReadyToLeaveCover()
	{
		return Time.time > enemy.minCoverTime + enemy.runToCoverState.lastTimeTookCover;
	}

	private void ChangeCoverIfShould()
	{
		if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover)
			return;

		coverCheckTimer -= Time.deltaTime;

		if (coverCheckTimer < 0)
		{
			coverCheckTimer = 0.5f; // check if should change cover when player is in sight every 0.5 secs.

			if (ReadyToChangeCover())
			{
				if (enemy.CanGetCover())
					stateMachine.ChangeState(enemy.runToCoverState);
			}
		}
	}

	private bool ReadyToChangeCover()
	{
		bool inDanger = IsPlayerInClearSight() || IsPlayerClose();
		bool advanceTimeIsOver = Time.time > enemy.advanceToPlayerState.lastTimeAdvanced + enemy.advanceDuration;

		return inDanger && advanceTimeIsOver;
	}

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

	private void SetupValuesForFirstAttack()
	{
		if (firstTimeAttack)
		{
			firstTimeAttack = false;
			bulletsPerShot = enemy.weaponData.bulletsToShoot;
			weaponCooldown = enemy.weaponData.GetWeaponCooldown();
		}
	}
	#endregion
}
