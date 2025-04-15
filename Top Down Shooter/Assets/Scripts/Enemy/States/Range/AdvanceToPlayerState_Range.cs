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
	}

	public override void Update()
	{
		base.Update();

		playerPos = enemy.playerTransform.position;
		enemy.UpdateAimPosition();

		enemy.agent.SetDestination(playerPos);
		enemy.FaceTarget(enemy.agent.steeringTarget);

		if (CanEnterBattleState())
			stateMachine.ChangeState(enemy.battleState);
	}

	public override void Exit()
	{
		base.Exit();

		lastTimeAdvanced = Time.time;
	}

	private bool CanEnterBattleState()
	{
		return Vector3.Distance(enemy.transform.position, playerPos) < enemy.advanceStoppingDistance && enemy.IsSeeingPlayer();
	}
}
