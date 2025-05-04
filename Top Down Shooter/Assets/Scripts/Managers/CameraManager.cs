using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager instance;

	private CinemachineVirtualCamera virtualCamera;
	private CinemachineFramingTransposer transposer;


	[SerializeField] private bool canChangeCameraDistance;
	private float targetCameraDistance;
	[SerializeField] private float distanceChangeRate;
	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
		{
			Debug.LogWarning("You had more than one Camera Manager");
			Destroy(gameObject);
		}

		virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
		transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
	}
	private void Update()
	{
		UpdateCameraDistance();
	}

	private void UpdateCameraDistance()
	{
		if (!canChangeCameraDistance)
			return;

		float currentDistance = transposer.m_CameraDistance;

		if (Mathf.Abs(targetCameraDistance - currentDistance) > 0.1f)
			transposer.m_CameraDistance = Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
	}

	public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;
}
