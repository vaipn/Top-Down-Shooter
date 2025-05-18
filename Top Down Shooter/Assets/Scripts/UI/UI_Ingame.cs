using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
	[Header("Health")]
    [SerializeField] private Image healthBarFill;


	[Header("Weapons")]
	[SerializeField] private UI_WeaponSlot[] weaponSlots_UI;

	private void Awake()
	{
		weaponSlots_UI = GetComponentsInChildren<UI_WeaponSlot>();
	}
	public void UpdateWeaponUI(List<Weapon> weaponSlots, Weapon currentWeapon)
	{
		// try to update each weapon slot from first one to last one
		for (int i = 0; i < weaponSlots_UI.Length; i++)
		{
			// to be sure we are updating slots only when we actually have weapons in the weapon slot of the player
			if (i < weaponSlots.Count)
			{
				bool isActiveWeapon = weaponSlots[i] == currentWeapon ? true : false;
				weaponSlots_UI[i].UpdateWeaponSlot(weaponSlots[i], isActiveWeapon);
			}
			else
			{
				weaponSlots_UI[i].UpdateWeaponSlot(null, false);
			}
		}
	}
	public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthBarFill.fillAmount = currentHealth/maxHealth;
		healthBarFill.color = GetHealthColor(healthBarFill.fillAmount);
    }

	private Color GetHealthColor(float value)
	{
		// Green -> Yellow -> Red
		if (value > 0.5f)
		{
			// Green to Yellow (value: 0.5 to 1.0)
			float t = (value - 0.5f) * 2f; // Normalize to 0–1
			return Color.Lerp(Color.yellow, Color.green, t);
		}
		else
		{
			// Yellow to Red (value: 0 to 0.5)
			float t = value * 2f; // Normalize to 0–1
			return Color.Lerp(Color.red, Color.yellow, t); // Lerp from red to yellow
		}
	}
}
