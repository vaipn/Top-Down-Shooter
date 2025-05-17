using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject_ObjectDeliveryZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		ObjectToDeliver objectToDeliver = other.GetComponent<ObjectToDeliver>();

		if (objectToDeliver != null)
			objectToDeliver.GetComponent<MissionObject_ObjectToDeliver>().InvokeOnObjectDelivery();
	}
}
