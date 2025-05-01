using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
	private Player player;
	private const float REFERENCE_BULLET_SPEED = 500; // This is the default speed from which our mass formula is derived

	[SerializeField] private LayerMask whatIsAlly;
	[Space]
	[SerializeField] private WeaponData defaultWeaponData;
	[SerializeField] private Weapon currentWeapon;
	private bool weaponReady;
	private bool isShooting;


	[Header("Bullet details")]
	[SerializeField] private float bulletImpactForce = 100;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float bulletSpeed;


	[SerializeField] private Transform weaponHolder;

	[Header("Inventory")]
	[SerializeField] private List<Weapon> weaponSlots;

	[SerializeField] private GameObject weaponPickupPrefab;

	private void Start()
	{
		player = GetComponent<Player>();

		AssignInputEvents();

		Invoke("EquipStartingWeapon", 0.1f);
	}

	private void Update()
	{
		if (isShooting)
			Shoot();
	}

	#region Slots Management - Equip/Drop/Pickup/Ready weapon
	private void EquipStartingWeapon()
	{
		weaponSlots[0] = new Weapon(defaultWeaponData);

		EquipWeapon(0);
	}

	private void EquipWeapon(int i)
	{
		if (i > 0 && weaponSlots.Count == 1)
			return;

		SetWeaponReady(false);

		currentWeapon = weaponSlots[i];

		player.weaponVisuals.PlayWeaponEquipAnimation();

		CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
	}


	private void DropWeapon()
	{
		if (HasOnlyOneWeapon())
			return;

		CreateWeaponOnTheGround();

		weaponSlots.Remove(currentWeapon);

		EquipWeapon(0); // weapon slots would have had only one element left
	}

	private void CreateWeaponOnTheGround()
	{
		GameObject droppedWeapon = ObjectPool.instance.GetObjectFromPool(weaponPickupPrefab, transform);
		droppedWeapon.GetComponent<PickupWeapon>()?.SetupPickupWeapon(currentWeapon, transform);
	}

	public void PickupWeapon(Weapon newWeapon)
	{
		if (WeaponInSlots(newWeapon.weaponType) != null)
		{
			WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
			return;
		}


		if (weaponSlots.Count >= 2 && newWeapon.weaponType != currentWeapon.weaponType) // there should only be max of 2 weapons equipped
		{
			int weaponIndex = weaponSlots.IndexOf(currentWeapon);

			player.weaponVisuals.SwitchOffWeaponModelsObjects();
			weaponSlots[weaponIndex] = newWeapon;

			CreateWeaponOnTheGround();
			EquipWeapon(weaponIndex);
			return;
		}

		weaponSlots.Add(newWeapon);
		player.weaponVisuals.SwitchOnBackupWeaponModelObject();
	}

	public Weapon WeaponInSlots(WeaponType weaponType)
	{
		foreach (Weapon weapon in weaponSlots)
		{
			if (weapon.weaponType == weaponType)
				return weapon;
		}

		return null;
	}

	public void SetWeaponReady(bool ready) => weaponReady = ready;
	public bool WeaponReady() => weaponReady;
	#endregion

	IEnumerator BurstShoot()
	{
		SetWeaponReady(false);

		for (int i = 1;	i <= currentWeapon.bulletsPerShot; i++)
		{
			ShootSingleBullet();

			yield return new WaitForSeconds(currentWeapon.burstFireDelay);
		}

		SetWeaponReady(true);
	}
	private void Shoot()
	{
		if (!WeaponReady())
			return;

		if (!currentWeapon.CanShoot())
			return;

		if (currentWeapon.shootType == ShootType.Single)
			isShooting = false;

		player.weaponVisuals.PlayShootAnimation();

		if (currentWeapon.BurstActivated())
		{
			StartCoroutine(BurstShoot());
			return;
		}

		ShootSingleBullet();
		TriggerEnemyDodge();
	}

	private void ShootSingleBullet()
	{
		currentWeapon.bulletsInMagazine--;

		GameObject newBullet = ObjectPool.instance.GetObjectFromPool(bulletPrefab, GunPoint());
		newBullet.transform.rotation = Quaternion.LookRotation(BulletDirection());


		Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

		Bullet bulletScript = newBullet.GetComponent<Bullet>();
		bulletScript.BulletSetup(whatIsAlly, currentWeapon.gunShotDistance, bulletImpactForce);

		Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

		rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
		rbNewBullet.velocity = bulletsDirection * bulletSpeed * Time.deltaTime;
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

	private void TriggerEnemyDodge()
	{
		// check if enemy is in the way of bullet fired. if present, trigger dodgeRoll
		Vector3 rayOrigin = GunPoint().position;
		Vector3 rayDirection = BulletDirection();

		if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
		{
			EnemyMelee enemyMelee = hit.collider.gameObject.GetComponentInParent<EnemyMelee>();

			if (enemyMelee != null)
			{
				enemyMelee.ActivateDodgeRoll();
			}
		}
	}

	#region Input Events
	private void AssignInputEvents()
	{
		PlayerControls controls = player.controls;


		controls.Character.Fire.performed += context => isShooting = true;
		controls.Character.Fire.canceled += context => isShooting = false;
		controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
		controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
		controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
		controls.Character.Reload.performed += context =>
		{
			if (currentWeapon.CanReload() && WeaponReady())
				Reload();
		};
		controls.Character.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurst();

	}
	#endregion
}
