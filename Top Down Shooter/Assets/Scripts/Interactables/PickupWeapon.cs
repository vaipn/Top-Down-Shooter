using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Interactable
{
	[SerializeField] private WeaponData weaponData;
	[SerializeField] private Weapon weapon;

	[SerializeField] private BackupWeaponModel[] weaponModels;

	private bool oldWeapon;
	private void Start()
	{
		if (!oldWeapon)
			weapon = new Weapon(weaponData);


		SetupGameObject();
	}

	public void SetupPickupWeapon(Weapon weapon, Transform transform)
	{
		oldWeapon = true;

		this.weapon = weapon;
		weaponData = weapon.weaponData;

		this.transform.position = transform.position + new Vector3(0, 0.75f, 0);
	}

	[ContextMenu("Update Item Model")]
	public void SetupGameObject()
	{
		//Debug.Log("Update Game Object Called");
		gameObject.name = "Pickup Weapon - " + weaponData.weaponType.ToString();
		SetupWeaponModel();
	}


	
	private void SetupWeaponModel()
	{
		foreach (BackupWeaponModel weaponModel in weaponModels)
		{
			weaponModel.gameObject.SetActive(false);

			if (weaponModel.weaponType == weaponData.weaponType)
			{
				weaponModel.gameObject.SetActive(true);
				UpdateMeshAndMaterial(weaponModel.GetComponent<MeshRenderer>());
			}
		}
	}

	public override void Interaction()
	{
		base.Interaction();

		weaponController?.PickupWeapon(weapon);
		ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject, 0);
	}
}
