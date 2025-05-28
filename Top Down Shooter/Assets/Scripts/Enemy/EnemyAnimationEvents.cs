using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;
	private EnemyMelee enemyMelee;
	private EnemyBoss enemyBoss;

	private void Awake()
	{
		enemy = GetComponentInParent<Enemy>();
		enemyMelee = GetComponentInParent<EnemyMelee>();
	}

	public void AnimationTrigger() => enemy.AnimationTrigger();

	public void StartManualMovement() => enemy.ActivateManualMovement(true);
	public void StopManualMovement() => enemy.ActivateManualMovement(false);
	public void StartManualRotation() => enemy.ActivateManualRotation(true);
	public void StopManualRotation() => enemy.ActivateManualRotation(false);
	public void AbilityEvent() => enemy.AbilityTrigger();
	public void EnableIK() => enemy.enemyVisuals.EnableIK(true, true, 1.5f);
	public void BossJumpImpact()
	{
		if (enemyBoss == null)
			enemyBoss = GetComponentInParent<EnemyBoss>();

		enemyBoss.JumpImpact();
	}
	public void BeginMeleeAttackCheck()
	{
		enemy?.EnableMeleeAttackCheck(true);

		enemy?.audioManager.PlaySFX(enemyMelee?.meleeSFX.kickSwooshSFX, true);
	}
	public void FinishMeleeAttackCheck() => enemy?.EnableMeleeAttackCheck(false);
}
