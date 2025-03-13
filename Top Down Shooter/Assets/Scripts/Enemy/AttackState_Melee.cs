using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
	public EnemyMelee enemy;
	private EnemyState[] possibleStates = new EnemyState[2];
	public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
	}

	public override void Enter()
	{
		if (possibleStates[0] == null || possibleStates[1] == null)
		{
			possibleStates[0] = enemy.recoveryState;
			possibleStates[1] = enemy.chaseState;
		}

		base.Enter();
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();

		if (triggerCalled)
		{
			int randomIndex = Random.Range(0, possibleStates.Length);

			stateMachine.ChangeState(possibleStates[randomIndex]);
		}
	}
}
