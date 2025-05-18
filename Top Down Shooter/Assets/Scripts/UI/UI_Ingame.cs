using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;

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
