using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class WeaponData : ScriptableObject
{
	public string weaponName;

	[Header("Bullet details")]
	public int bulletDamage;

	[Header("Regular shot")]
	public WeaponType weaponType;
	public int bulletsPerShot = 1;
	public float fireRate;

	[Header("Ammo details")]
	public int bulletsInMagazine;
	public int magazineCapacity;
	public int totalReserveAmmo;

	[Header("Burst shot")]
	public bool burstAvailable;
	public bool burstActive;
	public int burstBulletsPerShot;
	public float burstFireRate;
	public float burstFireDelay = 0.1f;

	[Header("Weapon spread")]
	public float baseSpread;
	public float maximumSpread;
	public float spreadIncreaseRate = 0.15f;
	public float spreadCooldown = 1;

	[Header("Weapon generics")]
	public ShootType shootType;
	[Range(1,2)]
	public float reloadSpeed = 1;

	[Range(1, 2)]
	public float equipSpeed = 1;

	[Range(2, 25)]
	public float gunShotDistance;

	[Range(3, 15)]
	public float cameraDistance;

	[Header("UI elements")]
	public Sprite weaponIcon;
	public string weaponInfo;
}
