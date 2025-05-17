using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defence Mission", menuName = "Missions/Defence Mission")]
public class Mission_Defence : Mission
{
	[Header("Respawn details")]
	public int amountOfRespawnPoints = 2;
	public List<Transform> respawnPoints;
	private Vector3 defencePoint;

	public override void StartMission()
	{
		defencePoint = FindObjectOfType<MissionEndTrigger>().transform.position;
		respawnPoints = new List<Transform>(ClosestPoints(amountOfRespawnPoints));
	}
	public override bool MissionCompleted()
	{
		throw new System.NotImplementedException();
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
}
