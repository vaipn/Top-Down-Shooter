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
		weaponVisualController.ReturnRigWeightToOne();
		
		//refill bullets
		weaponController.CurrentWeapon().RefillBullets();
		
	}

	public void ReturnRigWeightToOne()
	{
		weaponVisualController.ReturnRigWeightToOne();
	}

	public void WeaponGrabIsOver()
	{
		weaponVisualController.FinishedGrabbingWeapon();
	}
}
