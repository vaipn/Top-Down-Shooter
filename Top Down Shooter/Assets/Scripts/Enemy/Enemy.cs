using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour //You have to attach this to an enemy object
{
    public EnemyStateMachine stateMachine {  get; private set; }
	public EnemyVisuals enemyVisuals { get; private set; }

	public Transform playerTransform { get; private set; }

    public LayerMask whatIsAlly;
    public LayerMask whatIsPlayer;

    public int healthPoint = 5;

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

    protected bool isMeleeAttackReady;
    public bool inBattleMode {  get; private set; }
    public Animator anim {  get; private set; }

    public NavMeshAgent agent {  get; private set; }

	public Ragdoll ragdoll { get; private set; }

    public EnemyHealth health { get; private set; }
	protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        ragdoll = GetComponent<Ragdoll>();
		enemyVisuals = GetComponent<EnemyVisuals>();
        health = GetComponent<EnemyHealth>();

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

    protected virtual void InitializePerk()
    {

    }

    protected bool ShouldEnterBattleMode()
    {
        if (IsPlayerInAggressionRange() && !inBattleMode)
        {
            return true;
        }
        return false;
	}

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public virtual void GetHit(int damage)
    {
        health.ReduceHealth(damage);

        if (health.ShouldDie())
            Die(); // this is going to call the Die that overrides the virtual Die in this script

        EnterBattleMode(); // this is going to call the EnterBattleMode in EnemyMelee
    }

    public virtual void Die()
    {

    }

	public void EnableMeleeAttackCheck(bool enable) => isMeleeAttackReady = enable;

	public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius, GameObject fx, int damage)
	{
		if (!isMeleeAttackReady)
			return;

		foreach (Transform attackPoint in damagePoints)
		{
			Collider[] detectedHits = Physics.OverlapSphere(attackPoint.position, attackCheckRadius, whatIsPlayer);

			for (int i = 0; i < detectedHits.Length; i++)
			{
				IDamagable damagable = detectedHits[i].GetComponent<IDamagable>();

				if (damagable != null)
				{
					damagable.TakeDamage(damage);
					isMeleeAttackReady = false;
					GameObject newAttackFx = ObjectPool.instance.GetObjectFromPool(fx, attackPoint);
					ObjectPool.instance.ReturnObjectToPoolWithDelay(newAttackFx, 1);
					return;
				}

			}

		}
	}
	public virtual void BulletImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        if (health.ShouldDie())
            StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f); // delay needed so it doesn't add force at the moment when it is enabling ragdoll (and setting some parameters - like isKinematic)

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

	public void FaceTarget(Vector3 target, float turnSpeed = 0)
	{
		Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

		Vector3 currentEulerAngles = transform.rotation.eulerAngles; //current rotation

        if (turnSpeed == 0)
            turnSpeed = this.turnSpeed;

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
    public void DisableHeldWeapon()
    {
		enemyVisuals.currentHeldWeaponModel.gameObject.SetActive(false);
	}
    public bool IsPlayerInAggressionRange() => Vector3.Distance(transform.position, playerTransform.position) < aggressionRange;

	protected virtual void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, aggressionRange);
	}
}
