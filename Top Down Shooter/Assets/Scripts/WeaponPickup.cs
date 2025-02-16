using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
	[SerializeField] private Weapon weapon;
	private void OnTriggerEnter(Collider other)
	{
		other.GetComponent<PlayerWeaponController>()?.PickupWeapon(weapon);
	}
}
