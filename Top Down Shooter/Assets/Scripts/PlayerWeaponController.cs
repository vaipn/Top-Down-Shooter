using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
	private Player player;
	[SerializeField] private Weapon currentWeapon;
	
	private const float REFERENCE_BULLET_SPEED = 500; // This is the default speed from which our mass formula is derived

	private Animator animator;

	[Header("Bullet details")]
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float bulletSpeed;
	[SerializeField] private Transform gunPoint;


	[SerializeField] private Transform weaponHolder;

	private void Start()
	{
		player = GetComponent<Player>();
		animator = GetComponentInChildren<Animator>();

		player.controls.Character.Fire.performed += context => Shoot();

		currentWeapon.ammo = currentWeapon.maxAmmo;
	}

	private void Shoot()
	{
		if (currentWeapon.ammo <= 0)
			return;

		currentWeapon.ammo--;

		GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(BulletDirection()));

		Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();


		rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
		rbNewBullet.velocity = BulletDirection() * bulletSpeed * Time.deltaTime;

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
