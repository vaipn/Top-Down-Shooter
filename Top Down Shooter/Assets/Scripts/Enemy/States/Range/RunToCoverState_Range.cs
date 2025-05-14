using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunToCoverState_Range : EnemyState
{
    private EnemyRange enemy;
	private Vector3 destination;

	public float lastTimeTookCover {  get; private set; }
	public RunToCoverState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();

		destination = enemy.currentCover.transform.position;


		enemy.enemyVisuals.EnableIK(true, false);

		enemy.agent.isStopped = false;
		enemy.agent.speed = enemy.chaseSpeed;

		enemy.agent.SetDestination(destination);
	}

	public override void Exit()
	{
		base.Exit();

		lastTimeTookCover = Time.time;
	}

	public override void Update()
	{
		base.Update();

		enemy.FaceTarget(enemy.agent.steeringTarget);

		if (Vector3.Distance(enemy.transform.position, destination) <= 0.5f)
			stateMachine.ChangeState(enemy.battleState);
	}
}
