using UnityEngine;

public class EnemyGrenade : MonoBehaviour
{
	private Rigidbody rb;

	private void Awake() => rb = GetComponent<Rigidbody>();

	public void SetupGrenade(Vector3 target, float timeToReachTarget)
	{
		rb.velocity = CalculateLaunchVelocity(target, timeToReachTarget);
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
}
