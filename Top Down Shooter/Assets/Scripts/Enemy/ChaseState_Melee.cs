using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseState_Melee : EnemyState
{
	private EnemyMelee enemy;
	private float lastTimeUpdatedDestination;

	public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
	}

	public override void Enter()
	{
		base.Enter();

		enemy.agent.speed = enemy.chaseSpeed;

		enemy.agent.isStopped = false;

		Debug.Log("I enter Chase state");
	}

	public override void Exit()
	{
		base.Exit();

		Debug.Log("I exit Chase state");
	}

	public override void Update()
	{
		base.Update();

		if (enemy.PlayerInAttackRange())
			stateMachine.ChangeState(enemy.attackState);

		enemy.transform.rotation = enemy.FaceTarget(enemy.agent.steeringTarget);

		if (CanUpdateDestination())
		{
			enemy.agent.destination = enemy.playerTransform.position;
		}
	}

	private bool CanUpdateDestination()
	{
		if (Time.time > lastTimeUpdatedDestination + 0.25f)
		{
			lastTimeUpdatedDestination = Time.time;
			return true;
		}

		return false;
	}
}
