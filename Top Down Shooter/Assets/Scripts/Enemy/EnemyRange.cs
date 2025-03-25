using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : Enemy
{
	public IdleState_Range idleState {  get; private set; }
	public MoveState_Range moveState { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		idleState = new IdleState_Range(this, stateMachine, "Idle");
		moveState = new MoveState_Range(this, stateMachine, "Move");
	}

	protected override void Start()
	{
		base.Start();

		stateMachine.Initialize(idleState);
	}

	protected override void Update()
	{
		base.Update();

		stateMachine.currentState.Update();
	}
}
