using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Interactable
{
	private PlayerWeaponController weaponController;

	[SerializeField] private WeaponData weaponData;

	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);

		if (weaponController == null)
			weaponController = other.GetComponent<PlayerWeaponController>();
	}

	public override void Interaction()
	{
		base.Interaction();

		weaponController?.PickupWeapon(weaponData);
	}
}
