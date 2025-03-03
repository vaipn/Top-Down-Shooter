using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable] // Makes class visible in the inspector
public class Weapon
{
	public WeaponType weaponType;

	#region Regular Mode Variables
	public ShootType shootType;
	private float defaultFireRate;
	public float fireRate = 1; // bullets per second
	public int bulletsPerShot { get; private set; }
	private float lastShootTime;
	#endregion

	#region Burst mode variables
	private bool burstAvailable;
	public bool burstActive;
	private int burstBulletsPerShot;
	private float burstFireRate;
	public float burstFireDelay {  get; private set; }
	#endregion

	[Header("Ammo details")]
    public int bulletsInMagazine;
	public int magazineCapacity;
    public int totalReserveAmmo;

	#region Weapon generic info variables
	public float reloadSpeed { get; private set; }
	public float equipSpeed { get; private set; }
	public float gunShotDistance { get; private set; }
	public float cameraDistance { get; private set; }
	#endregion

	#region Weapon spread variables
	private float baseSpread;
	private float currentSpread;
	private float maximumSpread = 7;
	private float spreadIncreaseRate = 0.15f;

	private float lastSpreadUpdateTime;
	private float spreadCooldown = 1;
	#endregion
	public Weapon(WeaponData weaponData)
	{
		weaponType = weaponData.weaponType;

		fireRate = weaponData.fireRate;
		bulletsPerShot = weaponData.bulletsPerShot;

		bulletsInMagazine = weaponData.bulletsInMagazine;
		magazineCapacity = weaponData.magazineCapacity;
		totalReserveAmmo = weaponData.totalReserveAmmo;

		burstAvailable = weaponData.burstAvailable;
		burstActive = weaponData.burstActive;
		burstBulletsPerShot = weaponData.burstBulletsPerShot;
		burstFireRate = weaponData.burstFireRate;
		burstFireDelay = weaponData.burstFireDelay;


		baseSpread = weaponData.baseSpread;
		maximumSpread = weaponData.maximumSpread;
		spreadIncreaseRate = weaponData.spreadIncreaseRate;
		spreadCooldown = weaponData.spreadCooldown;


		shootType = weaponData.shootType;
		reloadSpeed = weaponData.reloadSpeed;
		equipSpeed = weaponData.equipSpeed;
		gunShotDistance = weaponData.gunShotDistance;
		cameraDistance = weaponData.cameraDistance;

		defaultFireRate = fireRate;
	}

	#region Spread Methods
	public Vector3 ApplySpread(Vector3 originalDirection)
	{
		UpdateSpread();

		float randomizedValue = Random.Range(-currentSpread, currentSpread);

		Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

		return spreadRotation * originalDirection;
	}

	public void IncreaseSpread()
	{
		currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
	}

	private void UpdateSpread()
	{
		if (Time.time > lastSpreadUpdateTime + spreadCooldown)
			currentSpread = baseSpread;
		else
			IncreaseSpread();

		lastSpreadUpdateTime = Time.time;
	}
	#endregion

	#region Burst Methods
	public bool BurstActivated()
	{
		if (weaponType == WeaponType.ShotGun)
		{
			burstFireDelay = 0;
			return true;
		}

		return burstActive;
	}

	public void ToggleBurst()
	{
		if (!burstAvailable) 
			return;

		burstActive = !burstActive;

		if (burstActive)
		{
			bulletsPerShot = burstBulletsPerShot;
			fireRate = burstFireRate;
		}
		else
		{
			bulletsPerShot = 1;
			fireRate = defaultFireRate;
		}
	}

	#endregion
	public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

	private bool HaveEnoughBullets() => bulletsInMagazine > 0;

	private bool ReadyToFire()
	{
		if (Time.time > lastShootTime + 1 / fireRate)
		{
			lastShootTime = Time.time;
			return true;
		}

		return false;
	}

	#region Reload methods
	public bool CanReload()
	{
		if (bulletsInMagazine == magazineCapacity)
			return false;

		if (totalReserveAmmo > 0)
			return true;

		return false;
	}

	public void RefillBullets()
	{
		if (weaponType == WeaponType.Revolver)
			totalReserveAmmo += bulletsInMagazine; // keep bullets in magazine while reloading.
												   // You don't throw unused magazine in a revolver away,
												   // you just add more bullets to the ones in the magazine.

		int bulletsToReload = magazineCapacity;

		if (bulletsToReload > totalReserveAmmo)
			bulletsToReload = totalReserveAmmo;

		totalReserveAmmo -= bulletsToReload;
		bulletsInMagazine = bulletsToReload;

		if (totalReserveAmmo < 0)
			totalReserveAmmo = 0;
	}
	#endregion
}

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    ShotGun,
    Sniper
}

public enum ShootType
{
	Single,
	Auto
}
