using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Melee : EnemyState
{
	private EnemyMelee enemy;

	private Vector3 movementDirection;
	private const float MAX_MOVEMENT_DISTANCE = 20;
	private float walkSpeed;
	public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
	}

	public override void Enter()
	{
		base.Enter();

		enemy.HoldWeapon();

		walkSpeed = enemy.walkSpeed;
		movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
	}
	public override void Exit()
	{
		base.Exit();
		enemy.walkSpeed = walkSpeed;
	}

	public override void Update()
	{
		base.Update();

		if (enemy.ManualRotationActive())
		{
			movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
			enemy.FaceTarget(enemy.playerTransform.position);
		}

		if (enemy.ManualMovementActive())
			enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, movementDirection, walkSpeed * Time.deltaTime);

		if (triggerCalled)
			stateMachine.ChangeState(enemy.recoveryState);
	}

	public override void AbilityTrigger()
	{
		base.AbilityTrigger();

		GameObject newAxe = ObjectPool.instance.GetObjectFromPool(enemy.axePrefab);

		newAxe.transform.position = enemy.axeStartPoint.position;
		newAxe.GetComponent<EnemyThrowAxe>().AxeSetup(enemy.axeFlySpeed, enemy.playerTransform, enemy.axeAimTimer);

	}
}
