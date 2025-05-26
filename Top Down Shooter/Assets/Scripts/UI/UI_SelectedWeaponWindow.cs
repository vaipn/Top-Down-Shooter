using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedWeaponWindow : MonoBehaviour
{
    public WeaponData weaponData;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponInfo;

	private void Start()
	{
        weaponData = null; // so weapon selection works normally in non quick play mode
        UpdateSlotInfo(null);
	}

    public void SetWeaponSlot(WeaponData newWeaponData)
    {
        weaponData = newWeaponData;
        UpdateSlotInfo(weaponData);
    }

	public void UpdateSlotInfo(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            weaponIcon.color = Color.clear;
            weaponInfo.text = "Select a weapon...";
            return;
        }

        weaponIcon.color = Color.white;
        weaponIcon.sprite = weaponData.weaponIcon;
        weaponInfo.text = weaponData.weaponInfo;
    }

    public bool IsEmpty() => weaponData == null;
}
