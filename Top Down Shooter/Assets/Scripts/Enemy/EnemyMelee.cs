using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
	public string attackName;
	public float attackRange;
	public float attackMoveSpeed;
	public float attackIndex;
	[Range(1, 2)]
	public float animationSpeed;
	public AttackType_Melee attackType;
}
public enum AttackType_Melee { Close, Charge }
public enum EnemyMelee_Type { Regular, Shield, Dodger, AxeThrower }
public class EnemyMelee : Enemy
{
	#region States
	public IdleState_Melee idleState { get; private set; }
	public MoveState_Melee moveState { get; private set; }
	public RecoveryState_Melee recoveryState { get; private set; }
	public ChaseState_Melee chaseState { get; private set; }
	public AttackState_Melee attackState { get; private set; }
	public DeadState_Melee deadState { get; private set; }
	public AbilityState_Melee abilityState { get; private set; }
	#endregion

	private AnimationClip[] clips;

	[Header("Enemy Settings")]
	public EnemyMelee_Type meleeType;
	[SerializeField] private Transform shieldTransform;
	public float dodgeCooldown;
	private float lastTimeDodge = -10;

	[Header("Axe throw ability")]
	public GameObject axePrefab;
	public float axeFlySpeed;
	public float axeAimTimer;
	public float axeThrowCooldown;
	private float lastTimeAxeThrown;
	public Transform axeStartPoint;

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
		abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");
	}

	protected override void Start()
	{
		clips = anim.runtimeAnimatorController.animationClips;

		base.Start();

		stateMachine.Initialize(idleState);

		InitializeSpeciality();
	}

	protected override void Update()
	{
		base.Update();

		stateMachine.currentState.Update();

		if (ShouldEnterBattleMode())
			EnterBattleMode();
	}

	public override void EnterBattleMode()
	{
		if (inBattleMode)
			return;

		base.EnterBattleMode();

		stateMachine.ChangeState(recoveryState);
	}

	public override void AbilityTrigger()
	{
		base.AbilityTrigger();

		walkSpeed = walkSpeed * 0.6f;
		heldWeapon.gameObject.SetActive(false);
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
		if (meleeType != EnemyMelee_Type.Dodger)
			return;

		if (stateMachine.currentState != chaseState)
			return;

		if (Vector3.Distance(transform.position, playerTransform.position) < 2f)
			return;

		float dodgeAnimationDuration = GetAnimationClipDuration("Dodge Roll");

		if (Time.time > lastTimeDodge + dodgeCooldown + dodgeAnimationDuration)
		{
			lastTimeDodge = Time.time;
			anim.SetTrigger("DodgeRoll");
		}
	}

	public bool CanThrowAxe()
	{
		if (meleeType != EnemyMelee_Type.AxeThrower)
			return false;

		if (Time.time > lastTimeAxeThrown + axeThrowCooldown)
		{
			lastTimeAxeThrown = Time.time;
			return true;
		}
		return false;
	}

	private float GetAnimationClipDuration(string clipName)
	{
		foreach (AnimationClip clip in clips)
		{
			if (clip.name == clipName)
				return clip.length;
		}

		Debug.Log(clipName + " animation not found");
		return 0f;
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
