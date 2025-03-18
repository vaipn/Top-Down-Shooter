using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
	public string attackName;
	public float attackRange;
	public float attackMoveSpeed;
	public float attackIndex;
	[Range (1, 2)]
	public float animationSpeed;
	public AttackType_Melee attackType;
}
public enum AttackType_Melee { Close, Charge}
public enum EnemyMelee_Type { Regular, Shield}
public class EnemyMelee : Enemy
{
    public IdleState_Melee idleState {  get; private set; }
    public MoveState_Melee moveState { get; private set; }
	public RecoveryState_Melee recoveryState { get; private set; }
	public ChaseState_Melee chaseState { get; private set; }
	public AttackState_Melee attackState { get; private set; }
	public DeadState_Melee deadState { get; private set; }

	[Header("Enemy Settings")]
	public EnemyMelee_Type meleeType;
	[SerializeField] private Transform shieldTransform;

	[Header("Attack data")]
	public AttackData attackData;
	public List<AttackData> attackList;

	[SerializeField] private Transform sheathedWeapon;
	[SerializeField] private Transform heldWeapon;

	protected override void Awake()
	{
		base.Awake();

		idleState = new IdleState_Melee(this, stateMachine, "Idle");
		moveState = new MoveState_Melee(this, stateMachine, "Move");
		recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
		chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
		attackState = new AttackState_Melee(this, stateMachine, "Attack");
		deadState = new DeadState_Melee(this, stateMachine, "Idle"); // Idle anim is just a placeholder (ragdoll is used)
	}

	protected override void Start()
	{
		base.Start();

		stateMachine.Initialize(idleState);

		InitializeSpeciality();
	}

	protected override void Update()
	{
		base.Update();

		stateMachine.currentState.Update();
	}

	private void InitializeSpeciality()
	{
		if (meleeType == EnemyMelee_Type.Shield)
		{
			anim.SetFloat("ChaseIndex", 1);
			shieldTransform.gameObject.SetActive(true);
		}
	}

	public override void GetHit()
	{
		base.GetHit();

		if (healthPoint <= 0)
			stateMachine.ChangeState(deadState);

	}

	public bool PlayerInAttackRange() => Vector3.Distance(transform.position, playerTransform.position) < attackData.attackRange;

	public void ActivateDodgeRoll()
	{
		anim.SetTrigger("DodgeRoll");
	}
	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
	}
	public void HoldWeapon()
	{
		sheathedWeapon.gameObject.SetActive(false);
		heldWeapon.gameObject.SetActive(true);
	}
	public void SheathWeapon()
	{
		sheathedWeapon.gameObject.SetActive(true);
		heldWeapon.gameObject.SetActive(false);
	}
}
