using UnityEngine;

[System.Serializable] // Makes class visible in the inspector
public class Weapon
{
    public WeaponType weaponType;
    public int bulletsInMagazine;
	public int magazineCapacity;
    public int totalReserveAmmo;

	[Range(1,2)]
	public float reloadSpeed = 1;

	[Range(1,2)]
	public float equipSpeed = 1;
    public bool CanShoot()
	{
		return HaveEnoughBullets();
	}

	private bool HaveEnoughBullets()
	{
		if (bulletsInMagazine > 0)
		{
			bulletsInMagazine--;
			return true;
		}

		return false;
	}

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
}

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    ShotGun,
    Sniper
}
