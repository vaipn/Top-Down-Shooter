using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour //You have to attach this to an enemy object
{
    public EnemyStateMachine stateMachine {  get; private set; }
    public EnemyState idleState { get; private set; }
    public EnemyState moveState { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new EnemyStateMachine();

        idleState = new EnemyState(this, stateMachine, "Idle");
        moveState = new EnemyState(this, stateMachine, "Move");

        stateMachine.Initialize(idleState);
    }

   
    void Update()
    {
        stateMachine.currentState.Update();

        if (Input.GetKeyDown(KeyCode.V))
            stateMachine.ChangeState(idleState);

        if (Input.GetKeyDown(KeyCode.C))
            stateMachine.ChangeState(moveState);
    }
}
