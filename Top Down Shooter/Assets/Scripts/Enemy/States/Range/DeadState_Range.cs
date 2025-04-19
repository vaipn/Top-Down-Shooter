using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Range : EnemyState
{
	private EnemyRange enemy;
	private bool interactionDisabled;
	public DeadState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyRange;
	}

	public override void Enter()
	{
		base.Enter();

		if (enemy.throwGrenadeState.finishedThrowingGrenade == false)
			enemy.ThrowGrenade();

		enemy.anim.enabled = false;
		enemy.agent.isStopped = true;

		enemy.ragdoll.RagdollActive(true);

		interactionDisabled = false;

		stateTimer = 3f;
	}

	public override void Update()
	{
		base.Update();

		DisableInteractionIfShould();
	}

	private void DisableInteractionIfShould()
	{
		if (stateTimer < 0 && !interactionDisabled)
		{
			interactionDisabled = true;

			enemy.ragdoll.RagdollActive(false);
			enemy.ragdoll.CollidersActive(false);
		}
	}
}
