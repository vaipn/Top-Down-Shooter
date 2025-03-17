using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Melee : EnemyState
{
	private EnemyMelee enemy;
	private EnemyRagdoll ragdoll;

	private bool interactionDisabled;
	public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
		ragdoll = enemy.GetComponent<EnemyRagdoll>();
	}

	public override void Enter()
	{
		base.Enter();

		enemy.anim.enabled = false;
		enemy.agent.isStopped = true;

		ragdoll.RagdollActive(true);

		interactionDisabled = false;

		stateTimer = 3f;
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();

		if (stateTimer < 0 && !interactionDisabled)
		{
			interactionDisabled = true;

			ragdoll.RagdollActive(false);
			ragdoll.CollidersActive(false);
		}
	}
}
