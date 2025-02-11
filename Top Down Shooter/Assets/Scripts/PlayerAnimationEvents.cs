using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals weaponVisualController;

	private void Start()
	{
		weaponVisualController = GetComponentInParent<PlayerWeaponVisuals>();
	}

	public void ReloadIsOver()
	{
		weaponVisualController.ReturnRigWeightToOne();

		//refill bullets
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
