using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private GameObject bulletImpactFX;


	private Rigidbody rb;
	private BoxCollider boxCollider;
	private MeshRenderer meshRenderer;
	private TrailRenderer trailRenderer;

	private float impactForce;
	private Vector3 startPosition;
	private float flyDistance;
	private bool bulletDisabled;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
		meshRenderer = GetComponent<MeshRenderer>();
		trailRenderer = GetComponent<TrailRenderer>();
	}

	public void BulletSetup(float flyDistance = 100, float impactForce = 100)
	{
		bulletDisabled = false;
		boxCollider.enabled = true;
		meshRenderer.enabled = true;

		trailRenderer.Clear();
		trailRenderer.time = 0.25f;
		startPosition = transform.position;
		this.flyDistance = flyDistance /*+ 0.5f*/; // +0.5 because of tipLength that is 0.5 
		this.impactForce = impactForce;
	}

	protected virtual void Update()
	{
		FadeTrailIfNeeded();
		DisableBulletIfNeeded();
		ReturnBulletToPoolIfNeeded();
	}

	protected void ReturnBulletToPoolIfNeeded()
	{
		if (trailRenderer.time < 0)
			ReturnBulletToPool();
	}

	protected void DisableBulletIfNeeded()
	{
		if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
		{
			boxCollider.enabled = false;
			meshRenderer.enabled = false;
			bulletDisabled = true;
		}
	}

	protected void FadeTrailIfNeeded()
	{
		if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
			trailRenderer.time -= 2 * Time.deltaTime;
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		CreateImpactFX();
		//rb.constraints = RigidbodyConstraints.FreezeAll;
		ReturnBulletToPool();

		IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
		damagable?.TakeDamage();

		Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
		EnemyShield shield = collision.gameObject.GetComponent<EnemyShield>();

		if (shield != null)
		{
			shield.ReduceDurability();
			return;
		}

		if (enemy != null)
		{
			Vector3 force = rb.velocity.normalized * impactForce; // direction * impactForce
			Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

			enemy.GetHit();
			enemy.DeathImpact(force, collision.contacts[0].point, hitRigidbody);

		}
	}

	protected void ReturnBulletToPool() => ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject);

	protected void CreateImpactFX()
	{
		GameObject newImpactFX = ObjectPool.instance.GetObjectFromPool(bulletImpactFX, transform);
		ObjectPool.instance.ReturnObjectToPoolWithDelay(newImpactFX, 1);
	}
}
