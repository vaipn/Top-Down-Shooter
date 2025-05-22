using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

	public Mission currentMission;

	public bool startedMission;

	private void Awake()
	{
		instance = this;
	}
	private void Update()
	{
		if (startedMission)
			currentMission?.UpdateMission();
	}

	public void SetMission(Mission newMission)
	{
		currentMission = newMission;
	}

	public void StartMission()
	{
		currentMission.StartMission();
		startedMission = true;
	}
	public bool MissionCompleted() => currentMission.MissionCompleted();
}
