using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Missions/Timer mission")]
public class Mission_Timer : Mission
{
	public float time;

	private float currentTime = 0.0001f; // small number so it doesn't sometime say game over at start
	public override void StartMission()
	{
		currentTime = time;
	}

	public override void UpdateMission()
	{
		currentTime -= Time.deltaTime;

		if (currentTime < 0)
			Debug.Log("Game Over");

		string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");

		string missionText = "Get to evacuation point before plane takes off.";
		string missionDetails = "Time left: " + timeText;

		UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
	}

	public override bool MissionCompleted()
	{
		return currentTime > 0;
	}
}
