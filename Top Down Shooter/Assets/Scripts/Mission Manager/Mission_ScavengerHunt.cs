using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hunt Mission", menuName = "Missions/Hunt Mission")]
public class Mission_ScavengerHunt : Mission
{
	public int amountToKill = 12;
	public EnemyType enemyType;

	private int killsToGo;
	public override void StartMission()
	{
		killsToGo = amountToKill;
		UpdateMissionUI();

		MissionObject_HuntTarget.OnTargetKilled += ReduceKillsToGo;

		List<Enemy> validEnemies = new List<Enemy>();

		if (enemyType == EnemyType.Random)
			validEnemies = LevelGenerator.instance.GetEnemyList();
		else
		{
			foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
			{
				if (enemy.enemyType == enemyType)
					validEnemies.Add(enemy);
			}
		}
		

		for (int i = 0; i < amountToKill; i++)
		{
			if (validEnemies.Count <= 0)
				return;

			int randomIndex = Random.Range(0, validEnemies.Count);
			validEnemies[randomIndex].AddComponent<MissionObject_HuntTarget>();
			validEnemies.RemoveAt(randomIndex);
		}
	}

	public override bool MissionCompleted()
	{
		return killsToGo <= 0;
	}

	private void ReduceKillsToGo()
	{
		killsToGo--;
		UpdateMissionUI();

		if (killsToGo <= 0)
		{
			UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point.");
			MissionObject_HuntTarget.OnTargetKilled -= ReduceKillsToGo;
		}
	}

	private void UpdateMissionUI()
	{
		string missionText = missionDescription;
		string missionDetails = "Kills left: " + killsToGo;

		UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
	}
}
