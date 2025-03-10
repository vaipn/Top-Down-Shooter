using UnityEngine;

public class MoveState_Melee : EnemyState
{
	private EnemyMelee enemy;

	private Vector3 destination;
	public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
	}

	public override void Enter()
	{
		base.Enter();

		destination = enemy.GetPatrolDestination();

		enemy.agent.SetDestination(destination);
	}

	public override void Exit()
	{
		base.Exit();

		Debug.Log("I exit move state");
	}

	public override void Update()
	{
		base.Update();

		if (enemy.PlayerInAggressionRange())
		{
			stateMachine.ChangeState(enemy.recoveryState);
			return;
		}

		enemy.transform.rotation = enemy.FaceTarget(enemy.agent.steeringTarget);

		if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.05f)
			stateMachine.ChangeState(enemy.idleState);
	}
}
