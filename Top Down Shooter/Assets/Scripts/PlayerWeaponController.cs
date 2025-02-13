using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
	private Animator animator;

	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float bulletSpeed;
	[SerializeField] private Transform gunPoint;
	[SerializeField] private Transform weaponHolder;
	[SerializeField] private Transform aim;

	private void Start()
	{
		player = GetComponent<Player>();
		animator = GetComponentInChildren<Animator>();

		player.controls.Character.Fire.performed += context => Shoot();
	}

	private void Shoot()
	{
		GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(BulletDirection()));

		newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed * Time.deltaTime;

		Destroy(newBullet, 10);

		animator.SetTrigger("Fire");
	}

	private Vector3 BulletDirection()
	{
		Vector3 direction = (aim.position - gunPoint.position).normalized;
		

		if (!player.aim.CanAimPrecisely())
			direction.y = 0;

		weaponHolder.LookAt(aim);
		gunPoint.LookAt(aim);

		return direction;
	}
}
