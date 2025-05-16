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

		MissionObject_HuntTarget.OnTargetKilled += ReduceKillsToGo;

		List<Enemy> validEnemies = new List<Enemy>();

		foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
		{
			if (enemy.enemyType == enemyType)
				validEnemies.Add(enemy);
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

		if (killsToGo <= 0)
			MissionObject_HuntTarget.OnTargetKilled -= ReduceKillsToGo;
	}
}
