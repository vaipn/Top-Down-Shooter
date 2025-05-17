using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defence Mission", menuName = "Missions/Defence Mission")]
public class Mission_Defence : Mission
{
	public bool defenceBegun = false;

	[Header("Cooldown and duration")]
	public float defenceDuration = 120;
	private float defenceTimer;
	public float waveCooldown = 15;
	private float waveTimer;


	[Header("Respawn details")]
	public int amountOfRespawnPoints = 2;
	public List<Transform> respawnPoints;
	private Vector3 defencePoint;
	[Space]

	public int enemiesPerWave;
	public GameObject[] possibleEnemies;

	// Because changing defenceBegun bool with script will save the current value in scriptable object, this OnEnable is needed,
	// so it resets every time the script is loaded or recompiled.
	private void OnEnable()
	{
		defenceBegun = false;
	}

	public override void StartMission()
	{
		defencePoint = FindObjectOfType<MissionEndTrigger>().transform.position;
		respawnPoints = new List<Transform>(ClosestPoints(amountOfRespawnPoints));
	}
	public override bool MissionCompleted()
	{
		// trigger defence mission when player gets to exit point destination (plane)
		if (!defenceBegun)
		{
			StartDefenceEvent();
			return false;
		}

		return defenceTimer < 0;
	}
	public override void UpdateMission()
	{
		if (!defenceBegun)
			return;

		defenceTimer -= Time.deltaTime;
		waveTimer -= Time.deltaTime;

		if (waveTimer < 0)
		{
			CreateNewEnemies(enemiesPerWave);
			waveTimer = waveCooldown;
		}
	}

	private void StartDefenceEvent()
	{
		waveTimer = 0.5f;
		defenceTimer = defenceDuration;
		defenceBegun = true;
	}

	private List<Transform> ClosestPoints(int amount)
	{
		List<Transform> closestPoints = new List<Transform>();
		List<MissionObject_EnemyRespawnPoint> allPoints = new List<MissionObject_EnemyRespawnPoint>(FindObjectsOfType<MissionObject_EnemyRespawnPoint>());

		while (allPoints.Count > 0 && closestPoints.Count < amount)
		{
			float shortestDistance = float.MaxValue;
			MissionObject_EnemyRespawnPoint closestPoint = null;

			foreach (var point in allPoints)
			{
				float distance = Vector3.Distance(point.transform.position, defencePoint);

				if (distance < shortestDistance)
				{
					shortestDistance = distance;
					closestPoint = point;
				}
			}

			if (closestPoint != null)
			{
				closestPoints.Add(closestPoint.transform);
				allPoints.Remove(closestPoint);
			}
		}

		return closestPoints;
	}

	private void CreateNewEnemies(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			int randomRespawnPointIndex = Random.Range(0, respawnPoints.Count);
			int randomEnemyIndex = Random.Range(0, possibleEnemies.Length);

			Transform randomRespawnPoint = respawnPoints[randomRespawnPointIndex];
			GameObject randomEnemy = possibleEnemies[randomEnemyIndex];

			randomEnemy.GetComponent<Enemy>().aggressionRange = 100; // so enemy always charge towards player

			ObjectPool.instance.GetObjectFromPool(randomEnemy, randomRespawnPoint);
		}
	}
}
