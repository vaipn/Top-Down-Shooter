using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Interactable
{
	private PlayerWeaponController weaponController;

	[SerializeField] private WeaponData weaponData;

	[SerializeField] private BackupWeaponModel[] weaponModels;


	private void Start()
	{
		UpdateGameObject();
	}

	[ContextMenu("Update Item Model")]
	public void UpdateGameObject()
	{
		gameObject.name = "Pickup Weapon - " + weaponData.weaponType.ToString();
		UpdateItemModel();
	}


	
	public void UpdateItemModel()
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
