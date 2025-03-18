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

	public float impactForce;
	private Vector3 startPosition;
	private float flyDistance;
	private bool bulletDisabled;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
		meshRenderer = GetComponent<MeshRenderer>();
		trailRenderer = GetComponent<TrailRenderer>();
	}

	public void BulletSetup(float flyDistance, float impactForce)
	{
		bulletDisabled = false;
		boxCollider.enabled = true;
		meshRenderer.enabled = true;
		trailRenderer.time = 0.25f;
		startPosition = transform.position;
		this.flyDistance = flyDistance /*+ 0.5f*/; // +0.5 because of tipLength that is 0.5 
		this.impactForce = impactForce;
	}

	private void Update()
	{
		FadeTrailIfNeeded();
		DisableBulletIfNeeded();
		ReturnBulletToPoolIfNeeded();
	}

	private void ReturnBulletToPoolIfNeeded()
	{
		if (trailRenderer.time < 0)
			ReturnBulletToPool();
	}

	private void DisableBulletIfNeeded()
	{
		if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
		{
			boxCollider.enabled = false;
			meshRenderer.enabled = false;
			bulletDisabled = true;
		}
	}

	private void FadeTrailIfNeeded()
	{
		if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
			trailRenderer.time -= 2 * Time.deltaTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		CreateImpactFX(collision);
		//rb.constraints = RigidbodyConstraints.FreezeAll;
		ReturnBulletToPool();


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
			enemy.HitImpact(force, collision.contacts[0].point, hitRigidbody);

		}
	}

	private void ReturnBulletToPool() => ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject);

	private void CreateImpactFX(Collision collision)
	{
		if (collision.contacts.Length > 0)
		{
			ContactPoint contact = collision.contacts[0]; // first point of contact

			GameObject newImpactFX = ObjectPool.instance.GetObjectFromPool(bulletImpactFX);
			newImpactFX.transform.position = contact.point;
			newImpactFX.transform.rotation = Quaternion.LookRotation(contact.normal);

			ObjectPool.instance.ReturnObjectToPoolWithDelay(newImpactFX, 1);
		}
	}
}
