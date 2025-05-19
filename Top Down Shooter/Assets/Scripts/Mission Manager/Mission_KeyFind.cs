using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key Mission", menuName = "Missions/Key Mission")]
public class Mission_KeyFind : Mission
{
	[SerializeField] private GameObject key;
	private bool keyFound;

	public override void StartMission()
	{
		MissionObject_Key.OnKeyPickedUp += PickUpKey;

		UI.instance.inGameUI.UpdateMissionInfo("Find and defeat the key-holder.", "Retrieve the key");

		// find random enemy
		Enemy enemy = LevelGenerator.instance.GetRandomEnemy();

		// give key to random enemy
		enemy.GetComponent<EnemyDropController>()?.GiveKey(key);


		// level up random enemy
		enemy.MakeEnemyVIP();
	}

	public override bool MissionCompleted()
	{
		return keyFound;
	}

	private void PickUpKey()
	{
		keyFound = true;
		MissionObject_Key.OnKeyPickedUp -= PickUpKey;

		UI.instance.inGameUI.UpdateMissionInfo("You've gotten the key!", "Get to the evacuation point.");
	}
}
