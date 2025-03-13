using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour //You have to attach this to an enemy object
{
    public EnemyStateMachine stateMachine {  get; private set; }
    public Transform playerTransform { get; private set; }


    [Header("Attack data")]
    public float attackRange;

    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float walkSpeed;
    public float chaseSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public float turnSpeed;
    public float aggressionRange;

    public Animator anim {  get; private set; }

    public NavMeshAgent agent {  get; private set; }
    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

		agent = GetComponent<NavMeshAgent>();
		anim = GetComponentInChildren<Animator>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
	}

    protected virtual void Start()
	{
		InitializePatrolPoints();

		
	}

	protected virtual void Update()
    {

    }

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, aggressionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, attackRange);
	}

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public bool PlayerInAggressionRange() => Vector3.Distance(transform.position, playerTransform.position) < aggressionRange;
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, playerTransform.position) < attackRange;
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
