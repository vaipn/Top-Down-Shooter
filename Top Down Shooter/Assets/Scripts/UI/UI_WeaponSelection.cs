using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WeaponSelection : MonoBehaviour
{
	[SerializeField] private GameObject nextUIToSwitchOn;
    public UI_SelectedWeaponWindow[] selectedWeaponWindow;

	[Header("Warning Info")]
	[SerializeField] private TextMeshProUGUI warningText;
	[SerializeField] private float disappearingSpeed = 0.25f;
	private float currentWarningAlpha;
	private float targetWarningAlpha;
	private void Start()
	{
		selectedWeaponWindow = GetComponentsInChildren<UI_SelectedWeaponWindow>();
	}
	private void Update()
	{
		if (currentWarningAlpha > targetWarningAlpha)
		{
			currentWarningAlpha -= Time.deltaTime * disappearingSpeed;
			warningText.color = new Color(1, 1, 1, currentWarningAlpha);
		}
	}
	
	public void ConfirmWeaponSelection()
	{
		if (AtLeastOneWeaponSelected())
		{
			UI.instance.StartLevelGeneration();
			UI.instance.SwitchTo(nextUIToSwitchOn);
		}
		else
			ShowWarningMessage("Select at least one weapon.");
	}

	private bool AtLeastOneWeaponSelected() => SelectedWeaponData().Count > 0;

	public List<WeaponData> SelectedWeaponData()
	{
		List<WeaponData> selectedData = new List<WeaponData>();

		foreach (UI_SelectedWeaponWindow weaponWindow in selectedWeaponWindow)
		{
			if (weaponWindow.weaponData != null) // if there is a weapon data on the window
				selectedData.Add(weaponWindow.weaponData);
		}
		return selectedData;
	}

	public UI_SelectedWeaponWindow FindEmptySlot()
	{
		for (int i = 0; i < selectedWeaponWindow.Length; i++)
		{
			if (selectedWeaponWindow[i].IsEmpty())
				return selectedWeaponWindow[i];
		}

		return null;
	}

	public UI_SelectedWeaponWindow FindSlotWithWeaponOfType(WeaponData weaponData)
	{
		for (int i = 0; i < selectedWeaponWindow.Length; i++)
		{
			if (selectedWeaponWindow[i].weaponData == weaponData)
				return selectedWeaponWindow[i];
		}

		return null;
	}

	public void ShowWarningMessage(string message)
	{
		warningText.color = Color.white;
		warningText.text = message;

		currentWarningAlpha = warningText.color.a;
		targetWarningAlpha = 0;
	}
}
