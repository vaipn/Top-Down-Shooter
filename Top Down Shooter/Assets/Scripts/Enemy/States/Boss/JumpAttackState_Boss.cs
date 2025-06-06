using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackState_Boss : EnemyState
{
	private EnemyBoss enemy;
	private Vector3 lastPlayerPos;

	private float jumpAttackMovementSpeed;
	public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyBoss;
	}

	public override void Enter()
	{
		base.Enter();

		lastPlayerPos = enemy.playerTransform.position;

		// so it doesn't slide before jumping
		enemy.agent.isStopped = true;
		enemy.agent.velocity = Vector3.zero;

		enemy.bossVisuals.PlaceLandingZone(lastPlayerPos + new Vector3(0, 0.1f, 0));
		enemy.bossVisuals.EnableWeaponTrails(true);

		float distanceToPlayer = Vector3.Distance(lastPlayerPos, enemy.transform.position);

		jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget;

		enemy.FaceTarget(lastPlayerPos, 1000);

		if (enemy.bossWeaponType == BossWeaponType.Hammer)
		{
			enemy.agent.isStopped = false;
			enemy.agent.speed = enemy.walkSpeed;
			enemy.agent.SetDestination(lastPlayerPos);
		}
	}

	public override void Update()
	{
		base.Update();

		Vector3 enemyPos = enemy.transform.position;

		enemy.agent.enabled = !enemy.ManualMovementActive(); // so it can jump over obstacles while in manual movement

		if (enemy.ManualMovementActive())
		{
			enemy.transform.position = Vector3.MoveTowards(enemyPos, lastPlayerPos, jumpAttackMovementSpeed * Time.deltaTime);
		}

		if (triggerCalled)
		{
			if (enemy.PlayerInAttackRange())
				stateMachine.ChangeState(enemy.idleState);
			else
				stateMachine.ChangeState(enemy.moveState);
		}
			
	}

	public override void Exit()
	{
		base.Exit();
		enemy.SetJumpAttackOnCooldown();
		enemy.bossVisuals.EnableWeaponTrails(false);
	}
}
