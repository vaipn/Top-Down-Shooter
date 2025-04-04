using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour //You have to attach this to an enemy object
{
    public EnemyStateMachine stateMachine {  get; private set; }
	public EnemyVisuals enemyVisuals { get; private set; }

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
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    public float aggressionRange;

    public bool inBattleMode {  get; private set; }
    public Animator anim {  get; private set; }

    public NavMeshAgent agent {  get; private set; }
    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

		enemyVisuals = GetComponent<EnemyVisuals>();

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
		if (ShouldEnterBattleMode())
			EnterBattleMode();
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

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f); // delay needed so it doesn't add force at the moment when it is enabling ragdoll (and setting some parameters - like isKinematic)

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

	public void FaceTarget(Vector3 target)
	{
		Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

		Vector3 currentEulerAngles = transform.rotation.eulerAngles; //current rotation

		float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime); // we only need to rotate on the y-axis

		transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
	}
	#region Animation Events
	public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;

    public bool ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }
	#endregion
	#region Patrol logic
	private void InitializePatrolPoints()
	{
		patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
	}

	public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }
	#endregion

	public void HoldWeapon()
	{
		enemyVisuals.currentSheathedWeaponModel.gameObject.SetActive(false);
		enemyVisuals.currentHeldWeaponModel.gameObject.SetActive(true);
	}
	public void SheathWeapon()
	{
		enemyVisuals.currentSheathedWeaponModel.gameObject.SetActive(true);
		enemyVisuals.currentHeldWeaponModel.gameObject.SetActive(false);
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, aggressionRange);
	}
}
