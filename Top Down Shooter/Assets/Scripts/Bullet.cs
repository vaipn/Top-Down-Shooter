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

	private int bulletDamage;
	private float impactForce;
	private Vector3 startPosition;
	private float flyDistance;
	private bool bulletDisabled;

	private LayerMask allyLayerMask;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
		meshRenderer = GetComponent<MeshRenderer>();
		trailRenderer = GetComponent<TrailRenderer>();
	}

	public void BulletSetup(LayerMask allyLayerMask, int damage, float flyDistance = 100, float impactForce = 100)
	{
		bulletDisabled = false;
		boxCollider.enabled = true;
		meshRenderer.enabled = true;

		trailRenderer.Clear();
		trailRenderer.time = 0.25f;
		startPosition = transform.position;
		this.flyDistance = flyDistance /*+ 0.5f*/; // +0.5 because of tipLength that is 0.5 
		this.impactForce = impactForce;
		this.allyLayerMask = allyLayerMask;
		this.bulletDamage = damage;
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
		if (!FriendlyFire())
		{
			if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0) // if the layer of the object the bullet collided with, is included in the layer(s) in the layerMask.
			{
				ReturnBulletToPool(10);
				return;
			}

		}

		CreateImpactFX();
		//rb.constraints = RigidbodyConstraints.FreezeAll;
		ReturnBulletToPool();

		IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
		damagable?.TakeDamage(bulletDamage);

		ApplyBulletImpactToEnemy(collision);
	}

	private void ApplyBulletImpactToEnemy(Collision collision)
	{
		Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

		if (enemy != null)
		{
			Vector3 force = rb.velocity.normalized * impactForce; // direction * impactForce
			Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

			enemy.BulletImpact(force, collision.contacts[0].point, hitRigidbody);
		}
	}

	protected void ReturnBulletToPool(float delay = 0.001f) => ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject, delay);

	protected void CreateImpactFX()
	{
		// Use this while testing impact SFX
		//GameObject newFx = Instantiate(bulletImpactFX);
		//newFx.transform.position = transform.position;

		//Destroy(newFx, 1);

		// Use this normally
		GameObject newImpactFX = ObjectPool.instance.GetObjectFromPool(bulletImpactFX, transform);
		ObjectPool.instance.ReturnObjectToPoolWithDelay(newImpactFX, 1);
	}

	private bool FriendlyFire() => GameManager.instance.friendlyFire;
}
