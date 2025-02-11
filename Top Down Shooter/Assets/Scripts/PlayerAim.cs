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

	[SerializeField] private Transform aim;
	public LayerMask aimLayerMask;

	private void Start()
	{
		player = GetComponent<Player>();
		AssignInputEvents();
	}
	private void Update()
	{
		aim.position = new Vector3(GetMousePosition().x, transform.position.y + 1.3f, GetMousePosition().z);
	}
	public Vector3 GetMousePosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(aimInput);

		if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
		{ 
			return hitInfo.point;
		}

		return Vector3.zero;
	}

	private void AssignInputEvents()
	{
		controls = player.controls;

		controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
		controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
	}
}
