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

    public float turnSpeed;

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

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngles = transform.rotation.eulerAngles; //current rotation

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime); // we only need to rotate on the y-axis

        return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }
}
