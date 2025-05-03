using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data/Range Weapon Data")]
public class EnemyRange_WeaponData : ScriptableObject
{
    [Header("Weapon details")]
    public EnemyRange_WeaponType weaponType;
    public float fireRate = 1f; // bullets per second

    public int bulletsToShoot = 1; // bullets to shoot before weapon goes on cooldown

	// weapon cooldown after all bullets are shot
	public float minWeaponCooldown = 2;
    public float maxWeaponCooldown = 3;

    [Header("Bullet details")]
    public int bulletDamage;
    [Space]
    public float bulletSpeed = 20;
    public float bulletSpread = 0.1f;

    public float GetWeaponCooldown() => Random.Range(minWeaponCooldown, maxWeaponCooldown);

	public Vector3 ApplySpread(Vector3 originalDirection)
	{
		float randomizedValue = Random.Range(-bulletSpread, bulletSpread);

		Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

		return spreadRotation * originalDirection;
	}
}
