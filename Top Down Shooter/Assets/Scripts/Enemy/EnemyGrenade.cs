using System;
using UnityEngine;

public class EnemyGrenade : MonoBehaviour
{
	[SerializeField] private GameObject explosionFx;
	[SerializeField] private float impactRadius;
	private float impactPower;
	[SerializeField] private float explosionUpwardsMultiplier = 1;
	private Rigidbody rb;
	private float timer;

	private void Awake() => rb = GetComponent<Rigidbody>();

	private void Update()
	{
		timer -= Time.deltaTime;

		if (timer < 0)
			Explode();
	}

	public void SetupGrenade(Vector3 target, float timeToReachTarget, float countdown, float impactPower)
	{
		rb.velocity = CalculateLaunchVelocity(target, timeToReachTarget);
		timer = countdown + timeToReachTarget;

		this.impactPower = impactPower;
	}

	private void Explode()
	{
		GameObject newExplosionFx = ObjectPool.instance.GetObjectFromPool(explosionFx);
		newExplosionFx.transform.position = transform.position;

		ObjectPool.instance.ReturnObjectToPoolWithDelay(newExplosionFx, 1);
		ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject);

		Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

		foreach (Collider collider in colliders)
		{
			Rigidbody rb = collider.GetComponent<Rigidbody>();

			if (rb != null)
				rb.AddExplosionForce(impactPower, transform.position, impactRadius, explosionUpwardsMultiplier, ForceMode.Impulse);
		}
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
