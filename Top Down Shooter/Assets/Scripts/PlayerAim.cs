using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    private Vector2 aimInput;

	[Header("Aim control")]
	[SerializeField] private Transform aim;

	[Header("Camera control")]
	[SerializeField] private Transform cameraTarget;
	[Range(0.5f, 1)]
	[SerializeField] private float minCameraDistance = 1.5f;
	[Range(1, 3f)]
	[SerializeField] private float maxCameraDistance = 4f;
	[SerializeField] private float cameraSensitivity = 4f;

	private RaycastHit lastKnownHitPoint;

	public LayerMask aimLayerMask;

	private void Start()
	{
		player = GetComponent<Player>();
		AssignInputEvents();
	}
	private void Update()
	{
		aim.position = GetMouseHitInfo().point;
		aim.position = new Vector3(aim.position.x, transform.position.y + 1.3f, aim.position.z);

		cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
	}

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

	public RaycastHit GetMouseHitInfo()
	{
		Ray ray = Camera.main.ScreenPointToRay(aimInput);

		if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
		{ 
			lastKnownHitPoint = hitInfo;
			return hitInfo;
		}

		return lastKnownHitPoint;
	}

	private void AssignInputEvents()
	{
		controls = player.controls;

		controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
		controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
	}
}
