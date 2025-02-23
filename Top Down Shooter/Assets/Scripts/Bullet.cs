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

	public void BulletSetup(float flyDistance)
	{
		bulletDisabled = false;
		boxCollider.enabled = true;
		meshRenderer.enabled = true;
		trailRenderer.time = 0.25f;
		startPosition = transform.position;
		this.flyDistance = flyDistance /*+ 0.5f*/; // +0.5 because of tipLength that is 0.5 
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
			ObjectPool.instance.ReturnBulletToQueue(gameObject);
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
		ObjectPool.instance.ReturnBulletToQueue(gameObject);
	}

	private void CreateImpactFX(Collision collision)
	{
		if (collision.contacts.Length > 0)
		{
			ContactPoint contact = collision.contacts[0]; // first point of contact

			GameObject newImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

			Destroy(newImpactFX, 1f);
		}
	}
}
