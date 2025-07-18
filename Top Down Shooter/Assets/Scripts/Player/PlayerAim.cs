using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
	private CameraManager cameraManager;
    private PlayerControls controls;

    private Vector2 mouseInput;

	[Header("Aim visual - Laser")]
	[SerializeField] private LineRenderer aimLaser;


	[Header("Aim Control")]
	[SerializeField] private float preciseAimCamDistance = 6;
	[SerializeField] private float regularAimCamDistance = 10;
	[SerializeField] private float camChangeRate = 5;

	[Header("Aim Setup")]
	[SerializeField] private Transform aim;
	[SerializeField] private bool isAimingPrecisely;
	[SerializeField] private float offsetChangeRate = 6;
	private float offsetY;

	[Header("Aim Layers")]
	public LayerMask preciseAimLayerMask;
	public LayerMask regularAimLayerMask;

	[Header("Camera Control")]
	[SerializeField] private Transform cameraTarget;
	[Range(0.5f, 1)]
	[SerializeField] private float minCameraDistance = 1.5f;
	[Range(1, 3f)]
	[SerializeField] private float maxCameraDistance = 4f;
	[SerializeField] private float cameraSensitivity = 4f;

	private RaycastHit lastKnownHitPoint;


	private void Start()
	{
		player = GetComponent<Player>();
		cameraManager = CameraManager.instance;

		AssignInputEvents();
	}
	private void Update()
	{
		if (player.health.isDead)
			return;

		if (player.controlsEnabled == false)
			return;

		//if (Input.GetKeyDown(KeyCode.P)) //precise aim toggle
		//	isAimingPrecisely = !isAimingPrecisely;

		//if (Input.GetKeyDown(KeyCode.L))
		//	isLockingToTarget = !isLockingToTarget;

		UpdateAimLaser();
		UpdateAimPosition();
		UpdateCameraPosition();
	}
	private void EnablePreciseAim(bool enable)
	{
		isAimingPrecisely = !isAimingPrecisely;
		Cursor.visible = false;

		if (enable)
		{
			cameraManager.ChangeCameraDistance(preciseAimCamDistance, camChangeRate);
			Time.timeScale = 0.9f;
		}
		else
		{
			cameraManager.ChangeCameraDistance(regularAimCamDistance, camChangeRate);
			Time.timeScale = 1;
		}
	}
	private void UpdateAimLaser()
	{
		aim.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		aimLaser.enabled = player.weaponController.WeaponReady();

		if (!aimLaser.enabled)
			return;

		//WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();

		
		//weaponModel.transform.LookAt(aim);
		//weaponModel.gunPoint.LookAt(aim);


		Transform gunPoint = player.weaponController.GunPoint();
		Vector3 laserDirection = player.weaponController.BulletDirection();

		float laserTipLength = 0.5f;
		float laserDistance = player.weaponController.CurrentWeapon().gunShotDistance;

		Vector3 endPoint = gunPoint.position + laserDirection * laserDistance;

		if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, laserDistance))
		{
			endPoint = hit.point;
			laserTipLength = 0;
		}

		aimLaser.SetPosition(0, gunPoint.position);
		aimLaser.SetPosition(1, endPoint);
		aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
	}
	private void UpdateAimPosition()
	{
		aim.position = GetMouseHitInfo().point;

		Vector3 newAimPosition = isAimingPrecisely ? aim.position : transform.position;

		aim.position = new Vector3(aim.position.x, newAimPosition.y + AdjustedOffsetY(), aim.position.z);
	}

	private float AdjustedOffsetY()
	{
		if (isAimingPrecisely)
			offsetY = Mathf.Lerp(offsetY, 0, Time.deltaTime * offsetChangeRate * 0.5f);
		else
			offsetY = Mathf.Lerp(offsetY, 1.3f, Time.deltaTime * offsetChangeRate);

		return offsetY;
	}


	//public Transform TargetToLock()
	//{
	//	Transform target = null;

	//	if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
	//	{
	//		target = GetMouseHitInfo().transform;
	//	}

	//	return target;
	//}
	public Transform Aim() => aim;

	public bool CanAimPrecisely() => isAimingPrecisely;


	public RaycastHit GetMouseHitInfo()
	{
		Ray ray = Camera.main.ScreenPointToRay(mouseInput);

		if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, preciseAimLayerMask))
		{ 
			lastKnownHitPoint = hitInfo;
			return hitInfo;
		}

		return lastKnownHitPoint;
	}
#region Camera Region
	private Vector3 DesiredCameraPosition()
	{
		float actualMaxCameraDistance = player.movement.moveInput.y < - 0.9f ? minCameraDistance : maxCameraDistance;

		Vector3 desiredCameraPosition = GetMouseHitInfo().point;
		Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

		float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
		float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);

		desiredCameraPosition = transform.position + aimDirection * clampedDistance;
		desiredCameraPosition.y = transform.position.y + 1.3f;

		return desiredCameraPosition;
		
	}
	private void UpdateCameraPosition()
	{
		bool canMoveCamera = Vector3.Distance(cameraTarget.position, DesiredCameraPosition()) > 1;

		if (canMoveCamera == false)
			return;

		cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
	}

#endregion
	private void AssignInputEvents()
	{
		controls = player.controls;

		controls.Character.PreciseAim.performed += context => EnablePreciseAim(true);
		controls.Character.PreciseAim.canceled += context => EnablePreciseAim(false);

		controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
		controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
	}
}
