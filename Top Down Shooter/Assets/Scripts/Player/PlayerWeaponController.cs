using System.Collections.Generic;
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

	[Header("Inventory")]
	[SerializeField] private List<Weapon> weaponSlots;

	private void Start()
	{
		player = GetComponent<Player>();
		animator = GetComponentInChildren<Animator>();

		AssignInputEvents();

		//currentWeapon.ammo = currentWeapon.maxAmmo;
		//currentWeapon = weaponSlots[0];
	}

	#region Slots Management - Equip/Drop/Pickup weapon
	private void EquipWeapon(int i)
	{
		currentWeapon = weaponSlots[i];

		player.weaponVisuals.PlayWeaponEquipAnimation();
	}

	private void DropWeapon()
	{
		if (HasOnlyOneWeapon())
			return;

		weaponSlots.Remove(currentWeapon);

		EquipWeapon(0); // weapon slots would have had only one element left
	}

	public void PickupWeapon(Weapon newWeapon)
	{
		if (weaponSlots.Count >= 2) // there should only be max of 2 weapons equipped
		{
			Debug.Log("No slots available");
			return;
		}

		weaponSlots.Add(newWeapon);
		player.weaponVisuals.SwitchOnBackupWeaponModelObject();
	}
	#endregion
	private void Shoot()
	{
		if (!currentWeapon.CanShoot())
			return;


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

	public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

	public Transform GunPoint() => gunPoint;

	public Weapon CurrentWeapon() => currentWeapon;
	public Weapon BackupWeapon()
	{
		foreach (Weapon weapon in weaponSlots)
		{
			if (weapon != currentWeapon)
				return weapon;
		}

		return null;
	}
	#region Input Events
	private void AssignInputEvents()
	{
		PlayerControls controls = player.controls;


		controls.Character.Fire.performed += context => Shoot();
		controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
		controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
		controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
		controls.Character.Reload.performed += context =>
		{
			if (currentWeapon.CanReload())
				player.weaponVisuals.PlayReloadAnimation();
		};

	}
	#endregion
}
