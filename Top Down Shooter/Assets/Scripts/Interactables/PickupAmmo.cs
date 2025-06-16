using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType { smallBox, bigBox };

[System.Serializable]
public struct AmmoData
{
	public WeaponType weaponType;
	[Range(10, 100)] public int minAmount;
	[Range(10, 100)] public int maxAmount;
}

public class PickupAmmo : Interactable
{
	[SerializeField] private AmmoBoxType boxType;

	[SerializeField] private List<AmmoData> smallBoxAmmo;
	[SerializeField] private List<AmmoData> bigBoxAmmo;

	[SerializeField] private GameObject[] boxModels;


	private void Start() => SetupBoxModel();

	public override void Interaction()
	{
		base.Interaction();

		List<AmmoData> currentAmmoList = smallBoxAmmo;

		if (boxType == AmmoBoxType.bigBox)
			currentAmmoList = bigBoxAmmo;

		foreach (AmmoData ammo in currentAmmoList)
		{
			Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);
			AddBulletsToWeapon(weapon, GetAmmoAmount(ammo));
		}

		Debug.Log("Added Ammo to weapon");
		ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject);

		GameManager.instance.player.weaponController.UpdateWeaponUI();
	}
	private int GetAmmoAmount(AmmoData ammoData)
	{
		float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
		float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

		float randomAmmoAmount = Random.Range(min, max);

		return Mathf.RoundToInt(randomAmmoAmount);
	}
	private void AddBulletsToWeapon(Weapon weapon, int amount)
	{
		if (weapon != null)
			weapon.totalReserveAmmo += amount;
	}

	private void SetupBoxModel()
	{
		for (int i = 0; i < boxModels.Length; i++)
		{
			boxModels[i].SetActive(false);

			if (i == ((int)boxType))
			{
				boxModels[i].SetActive(true);
				UpdateMeshAndMaterial(boxModels[i].GetComponent<MeshRenderer>());
			}
		}
	}
}

