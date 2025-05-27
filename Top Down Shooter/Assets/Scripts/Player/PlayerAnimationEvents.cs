using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals weaponVisualController;
	private PlayerWeaponController weaponController;

	private void Start()
	{
		weaponVisualController = GetComponentInParent<PlayerWeaponVisuals>();
		weaponController = GetComponentInParent<PlayerWeaponController>();
	}

	public void ReloadIsOver()
	{
		weaponController.SetWeaponReady(true);

		weaponVisualController.ReturnRigWeightToOne();

		weaponVisualController.CurrentWeaponModel().reloadSFX.Stop();

		//refill bullets
		weaponController.CurrentWeapon().RefillBullets();
		weaponController.UpdateWeaponUI();
		
	}

	public void ReturnRigWeightToOne()
	{
		weaponVisualController.ReturnRigWeightToOne();
	}

	public void WeaponEquipIsOver()
	{
		weaponController.SetWeaponReady(true);
	}

	public void SwitchOnWeaponModelObject() => weaponVisualController.SwitchOnCurrentWeaponModelObject();
}
