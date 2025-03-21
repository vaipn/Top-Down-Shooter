using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour //You have to attach this to an enemy object
{
    public EnemyStateMachine stateMachine {  get; private set; }
    public Transform playerTransform { get; private set; }


    [SerializeField] protected int healthPoint = 5;

    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float walkSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public float aggressionRange;

    public bool inBattleMode {  get; private set; }
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

    protected bool ShouldEnterBattleMode()
    {
        bool inAgressionRange = Vector3.Distance(transform.position, playerTransform.position) < aggressionRange;

        if (inAgressionRange && !inBattleMode)
        {
            return true;
        }
        return false;
	}

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public virtual void GetHit()
    {
        EnterBattleMode(); // this is going to call the EnterBattleMode in EnemyMelee
        healthPoint--;
    }

    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f); // delay needed so it doesn't add force at the moment when it is enabling ragdoll (and setting some parameters - like isKinematic)

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
	protected virtual void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, aggressionRange);
	}
    
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;

    public bool ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
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

    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngles = transform.rotation.eulerAngles; //current rotation

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime); // we only need to rotate on the y-axis

        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }


}
