using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Interactable
{
	private PlayerWeaponController weaponController;

	[SerializeField] private WeaponData weaponData;
	[SerializeField] private Weapon weapon;

	[SerializeField] private BackupWeaponModel[] weaponModels;

	private bool oldWeapon;
	private void Start()
	{
		if (!oldWeapon)
			weapon = new Weapon(weaponData);


		UpdateGameObject();
	}

	public void SetupPickupWeapon(Weapon weapon, Transform transform)
	{
		oldWeapon = true;

		this.weapon = weapon;
		weaponData = weapon.weaponData;

		this.transform.position = transform.position + new Vector3(0, 0.75f, 0);
	}

	[ContextMenu("Update Item Model")]
	public void UpdateGameObject()
	{
		Debug.Log("Update Game Object Called");
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

		weaponController?.PickupWeapon(weapon);
		ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject, 0);
	}
}
