using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceToPlayerState_Range : EnemyState
{
	private EnemyRange enemy;
	private Vector3 playerPos;

	public float lastTimeAdvanced {  get; private set; }
	public AdvanceToPlayerState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();

		enemy.enemyVisuals.EnableIK(true, false);

		enemy.agent.isStopped = false;
		enemy.agent.speed = enemy.advanceSpeed;

		if (enemy.IsUnstoppable())
		{
			enemy.enemyVisuals.EnableIK(true, false);
			stateTimer = enemy.advanceDuration;
		}
	}

	public override void Update()
	{
		base.Update();

		playerPos = enemy.playerTransform.position;
		enemy.UpdateAimPosition();

		enemy.agent.SetDestination(playerPos);
		enemy.FaceTarget(enemy.agent.steeringTarget);

		if (CanEnterBattleState() && enemy.IsSeeingPlayer())
			stateMachine.ChangeState(enemy.battleState);
	}

	public override void Exit()
	{
		base.Exit();

		lastTimeAdvanced = Time.time;
	}

	private bool CanEnterBattleState()
	{
		bool closeEnoughToPlayer = Vector3.Distance(enemy.transform.position, playerPos) < enemy.advanceStoppingDistance;
		
		if (enemy.IsUnstoppable())
			return closeEnoughToPlayer || stateTimer < 0;
		else
			return closeEnoughToPlayer;
	}
}
