using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WeaponSelectionButton : UI_Button
{
    private UI_WeaponSelection weaponSelectionUI;

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Image weaponIcon;

	private UI_SelectedWeaponWindow emptySlot;

	private Button thisButton;

	private void OnValidate()
	{
		gameObject.name = "Button - Select Weapon: " + weaponData.weaponType;
	}

	public override void Start()
	{
		base.Start();

		thisButton = this.GetComponent<Button>();

		weaponSelectionUI = GetComponentInParent<UI_WeaponSelection>();
		weaponIcon.sprite = weaponData.weaponIcon;
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		ReturnIfButtonUninteractable();

		base.OnPointerEnter(eventData);

		weaponIcon.color = Color.yellow;

		emptySlot = weaponSelectionUI.FindEmptySlot();
		emptySlot?.UpdateSlotInfo(weaponData);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		ReturnIfButtonUninteractable();

		base.OnPointerExit(eventData);

		weaponIcon.color = Color.white;

		emptySlot?.UpdateSlotInfo(null);
		emptySlot = null;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		ReturnIfButtonUninteractable();

		base.OnPointerDown(eventData);

		weaponIcon.color = Color.white;

		bool noMoreEmptySlots = weaponSelectionUI.FindEmptySlot() == null;
		bool noThisWeaponInSlots = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData) == null;

		if (noMoreEmptySlots && noThisWeaponInSlots)
		{
			weaponSelectionUI.ShowWarningMessage("No Empty Slots...");
			return;
		}

		UI_SelectedWeaponWindow slotWithThisWeapon = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData); // find the slot with the weapon of this button

		if (slotWithThisWeapon != null)
		{
			slotWithThisWeapon.SetWeaponSlot(null); // remove this weapon if it is already selected
		}
		else
		{
			emptySlot = weaponSelectionUI.FindEmptySlot(); // required because if it is only called in OnPointerEnter,
														   // there would be an error if you deselect a weapon and set
														   // it back without exiting and re-hovering over the button 
			emptySlot.SetWeaponSlot(weaponData);
		}

		emptySlot = null; // required, so OnPointerExit doesn't call UpdateSlotInfo(null) -- setting the slot info back to null
	}

	private void ReturnIfButtonUninteractable()
	{
		if (!thisButton.interactable)
			return;
	}
}
