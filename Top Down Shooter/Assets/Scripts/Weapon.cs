using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable] // Makes class visible in the inspector
public class Weapon
{
    public WeaponType weaponType;

	[Header("Shooting details")]
	public ShootType shootType;
	public float defaultFireRate;
	public float fireRate = 1; // bullets per second
	public float bulletsPerShot;
	private float lastShootTime;

	[Header("Burst details")]
	public bool burstAvailable;
	public bool burstActive;
	public int burstBulletsPerShot;
	public float burstFireRate;
	public float burstFireDelay;


	[Header("Ammo details")]
    public int bulletsInMagazine;
	public int magazineCapacity;
    public int totalReserveAmmo;

	[Range(1,2)]
	public float reloadSpeed = 1;

	[Range(1,2)]
	public float equipSpeed = 1;

	[Range(2, 12)]
	public float gunShotDistance;

	[Range(3, 8)]
	public float cameraDistance;

	[Header("Spread")]
	public float baseSpread;
	private float currentSpread;
	public float maximumSpread = 7;
	public float spreadIncreaseRate = 0.15f;

	private float lastSpreadUpdateTime;
	private float spreadCooldown = 1;


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
			return true;

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
