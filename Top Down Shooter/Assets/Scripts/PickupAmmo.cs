using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : Interactable
{
	public override void Interaction()
	{
		base.Interaction();

		Debug.Log("Added Ammo to weapon");
	}
}
