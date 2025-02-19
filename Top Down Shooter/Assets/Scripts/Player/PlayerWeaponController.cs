using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
	private Player player;
	[SerializeField] private Weapon currentWeapon;
	private bool weaponReady;
	
	private const float REFERENCE_BULLET_SPEED = 500; // This is the default speed from which our mass formula is derived

	private Animator animator;

	[Header("Bullet details")]
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float bulletSpeed;


	[SerializeField] private Transform weaponHolder;

	[Header("Inventory")]
	[SerializeField] private List<Weapon> weaponSlots;

	private void Start()
	{
		player = GetComponent<Player>();
		animator = GetComponentInChildren<Animator>();

		AssignInputEvents();

		Invoke("EquipStartingWeapon", 0.1f);
	}

	#region Slots Management - Equip/Drop/Pickup/Ready weapon
	private void EquipStartingWeapon() => EquipWeapon(0);
	private void EquipWeapon(int i)
	{
		SetWeaponReady(false);

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

	public void SetWeaponReady(bool ready) => weaponReady = ready;
	public bool WeaponReady() => weaponReady;
	#endregion
	private void Shoot()
	{
		if (!WeaponReady())
			return;

		if (!currentWeapon.CanShoot())
			return;


		GameObject newBullet = ObjectPool.instance.GetBulletFromQueue();
		newBullet.transform.position = GunPoint().position;
		newBullet.transform.rotation = Quaternion.LookRotation(BulletDirection());
		

		Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();


		rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
		rbNewBullet.velocity = BulletDirection() * bulletSpeed * Time.deltaTime;

		animator.SetTrigger("Fire");
	}
	private void Reload()
	{
		SetWeaponReady(false);
		player.weaponVisuals.PlayReloadAnimation();
	}

	public Vector3 BulletDirection()
	{
		Transform aim = player.aim.Aim();
		Vector3 direction = (aim.position - GunPoint().position).normalized;
		

		if (!player.aim.CanAimPrecisely() && player.aim.TargetToLock() == null)
			direction.y = 0;

		return direction;
	}

	public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

	public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

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
			if (currentWeapon.CanReload() && WeaponReady())
				Reload();
		};

	}
	#endregion
}
