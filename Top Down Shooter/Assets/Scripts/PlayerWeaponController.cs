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

	public Vector3 BulletDirection()
	{
		Transform aim = player.aim.Aim();
		Vector3 direction = (aim.position - gunPoint.position).normalized;
		

		if (!player.aim.CanAimPrecisely() && player.aim.TargetToLock() == null)
			direction.y = 0;

		// TODO: Find a better place for these
		//weaponHolder.LookAt(aim);
		//gunPoint.LookAt(aim);

		return direction;
	}

	public Transform GunPoint() => gunPoint;
}
