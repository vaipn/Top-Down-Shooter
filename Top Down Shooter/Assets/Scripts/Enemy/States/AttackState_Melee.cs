using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
	public EnemyMelee enemy;
	private EnemyState[] possibleStates = new EnemyState[2];

	private Vector3 attackDirection;
	private const float MAX_ATTACK_DISTANCE = 50f;
	private float attackMoveSpeed;
	public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
	{
		enemy = enemyBase as EnemyMelee;
	}

	public override void Enter()
	{
		base.Enter();
		enemy.HoldWeapon();
		enemy.enemyVisuals.EnableWeaponTrail(true);

		attackMoveSpeed = enemy.attackData.attackMoveSpeed;
		enemy.anim.SetFloat("AttackAnimationSpeed", enemy.attackData.animationSpeed);
		enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex);
		enemy.anim.SetFloat("SlashAttackIndex", Random.Range(0, 5));

		//if (possibleStates[0] == null || possibleStates[1] == null)
		//{
		//	possibleStates[0] = enemy.recoveryState;
		//	possibleStates[1] = enemy.chaseState;
		//}



		enemy.agent.isStopped = true;
		enemy.agent.velocity = Vector3.zero;

		attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);

		Debug.Log("I enter Attack state");
	}

	public override void Exit()
	{
		base.Exit();

		SetupNextAttack();

		enemy.enemyVisuals.EnableWeaponTrail(false);
		Debug.Log("I exit Attack state");
	}

	private void SetupNextAttack()
	{
		int recoveryIndex = PlayerClose() ? 1 : 0;

		enemy.anim.SetFloat("RecoveryIndex", recoveryIndex);

		enemy.attackData = UpdatedAttackData();
	}

	public override void Update()
	{
		base.Update();

		if (enemy.ManualRotationActive())
		{
			attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
			enemy.FaceTarget(enemy.playerTransform.position);
		}

		if (enemy.ManualMovementActive())
			enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);



		if (triggerCalled)
		{
			//int randomIndex = Random.Range(0, possibleStates.Length);

			if (enemy.PlayerInAttackRange())
				stateMachine.ChangeState(enemy.recoveryState);
			else
				stateMachine.ChangeState(enemy.chaseState);
		}
	}

	public bool PlayerClose() => Vector3.Distance(enemy.transform.position, enemy.playerTransform.position) <= 1;

	private AttackData UpdatedAttackData()
	{
		List<AttackData> validAttacks = new List<AttackData>(enemy.attackList);

		if (PlayerClose())
			validAttacks.RemoveAll(parameters => parameters.attackType == AttackType_Melee.Charge); //remove all elements that is of attacktype type Charge


		int randomIndex = Random.Range(0, validAttacks.Count);

		return validAttacks[randomIndex];
	}
}
