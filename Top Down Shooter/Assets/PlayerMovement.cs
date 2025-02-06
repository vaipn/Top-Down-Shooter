using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	private PlayerControls controls;
	private CharacterController characterController;

	[SerializeField] private Vector3 movementDirection;
	[SerializeField] private float movementSpeed;

	private Vector2 moveInput;
	private Vector2 aimInput;

	private void Awake()
	{
		controls = new PlayerControls();

		controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
		controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

		controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
		controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
	}

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		ApplyMovement();
	}

	private void ApplyMovement()
	{
		movementDirection = new Vector3(moveInput.x, 0, moveInput.y);

		if (movementDirection.magnitude > 0)
		{
			characterController.Move(movementDirection * Time.deltaTime * movementSpeed);
		}
	}

	private void OnEnable()
	{
		controls.Enable();
	}

	private void OnDisable()
	{
		controls.Disable();
	}
}
