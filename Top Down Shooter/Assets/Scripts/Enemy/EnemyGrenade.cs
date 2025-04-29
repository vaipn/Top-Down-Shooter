using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrenade : MonoBehaviour
{
	[SerializeField] private GameObject explosionFx;
	[SerializeField] private float impactRadius;
	private float impactPower;
	[SerializeField] private float explosionUpwardsMultiplier = 1;
	private Rigidbody rb;
	private float timer;

	private LayerMask allyLayerMask;
	private bool canExplode = true;

	private void Awake() => rb = GetComponent<Rigidbody>();

	private void Update()
	{
		timer -= Time.deltaTime;

		if (timer < 0 && canExplode)
			Explode();
	}

	public void SetupGrenade(LayerMask allyLayerMask, Vector3 target, float timeToReachTarget, float countdown, float impactPower)
	{
		canExplode = true;

		rb.velocity = CalculateLaunchVelocity(target, timeToReachTarget);
		timer = countdown + timeToReachTarget;

		this.impactPower = impactPower;
		this.allyLayerMask = allyLayerMask;
	}
	private bool IsTargetValid(Collider collider)
	{
		// if friendly fire is enabled, all colliders are valid targets
		if (GameManager.instance.friendlyFire)
			return true;

		// if collider has allyLayer, target is not valid
		if ((allyLayerMask.value & (1 << collider.gameObject.layer)) > 0)
			return false;

		return true;
	}
	private void Explode()
	{
		canExplode = false;

		PlayExplosionEffect();

		HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
		Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

		foreach (Collider collider in colliders)
		{
			if (!IsTargetValid(collider))
				continue;

			GameObject rootEntity = collider.transform.root.gameObject;
			if (uniqueEntities.Add(rootEntity) == false)
				continue;

			ApplyDamageTo(collider);

			ApplyExplosionForceTo(collider);
		}
	}

	private void ApplyExplosionForceTo(Collider collider)
	{
		Rigidbody rb = collider.GetComponent<Rigidbody>();

		if (rb != null)
			rb.AddExplosionForce(impactPower, transform.position, impactRadius, explosionUpwardsMultiplier, ForceMode.Impulse);
	}

	private static void ApplyDamageTo(Collider collider)
	{
		IDamagable damagable = collider.GetComponent<IDamagable>();
		damagable?.TakeDamage();
	}

	private void PlayExplosionEffect()
	{
		GameObject newExplosionFx = ObjectPool.instance.GetObjectFromPool(explosionFx, transform);

		ObjectPool.instance.ReturnObjectToPoolWithDelay(newExplosionFx, 1);
		ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject);
	}

	private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToReachTarget)
	{
		Vector3 direction = target - transform.position;
		Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

		Vector3 velocityXZ = directionXZ / timeToReachTarget;

		float velocityY = (direction.y - (Physics.gravity.y * Mathf.Pow(timeToReachTarget, 2)) / 2) / timeToReachTarget;

		Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY;

		return launchVelocity;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, impactRadius);
	}
}
