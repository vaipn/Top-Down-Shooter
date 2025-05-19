using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object Delivery Mission", menuName = "Missions/Object Delivery Mission")]
public class Mission_ObjectDelivery : Mission
{
	private bool objectWasDelivered;

	public string objectName;
	public override void StartMission()
	{
		FindObjectOfType<MissionObject_ObjectDeliveryZone>(true).gameObject.SetActive(true);

		string missionText = "Find " + objectName;
		string missionDetails = "Deliver it to the delivery zone";

		UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);


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

		UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point");
	}
}
