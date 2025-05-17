using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object Delivery Mission", menuName = "Missions/Object Delivery Mission")]
public class Mission_ObjectDelivery : Mission
{
	private bool objectWasDelivered;
	public override void StartMission()
	{
		objectWasDelivered = false;
		MissionObject_ObjectToDeliver.OnObjectDelivery += ObjectDeliveryCompleted;

		ObjectToDeliver[] objects = FindObjectsOfType<ObjectToDeliver>();

		foreach (ObjectToDeliver obj in objects)
		{
			obj.AddComponent<MissionObject_ObjectToDeliver>();
		}
	}

	public override bool MissionCompleted()
	{
		return objectWasDelivered;
	}

	private void ObjectDeliveryCompleted()
	{
		objectWasDelivered = true;
		MissionObject_ObjectToDeliver.OnObjectDelivery -= ObjectDeliveryCompleted;
	}
}
