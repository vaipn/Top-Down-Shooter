using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour //You have to attach this to an enemy object
{
    public EnemyStateMachine stateMachine {  get; private set; }


    [Header("Idle Info")]
    public float idleTime;
    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }
}
