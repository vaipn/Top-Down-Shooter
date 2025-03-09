using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour //You have to attach this to an enemy object
{
    public EnemyStateMachine stateMachine {  get; private set; }


    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float moveSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Animator anim {  get; private set; }

    public NavMeshAgent agent {  get; private set; }
    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        
    }

    protected virtual void Start()
	{
		InitializePatrolPoints();

		agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
	}

	protected virtual void Update()
    {

    }


	private void InitializePatrolPoints()
	{
		foreach (Transform t in patrolPoints)
			t.parent = null;
	}

	public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }
}
