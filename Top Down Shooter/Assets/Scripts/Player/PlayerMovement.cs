using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private PlayerControls controls;
	private Player player;
	private CharacterController characterController;


	private Animator animator;

	[Header("Movement info")]
	[SerializeField] private Vector3 movementDirection;
	[SerializeField] private float movementSpeed;
	[SerializeField] private float turnSpeed;
	[SerializeField] private float runSpeed;
	private float walkSpeed;
	private float verticalVelocity;
	private bool isRunning;

	public Vector2 moveInput { get; private set; }
	private void Start()
	{
		player = GetComponent<Player>();

		characterController = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();

		walkSpeed = movementSpeed;

		AssignInputEvents();
	}

	private void Update()
	{
		if (player.health.isDead)
			return;

		ApplyMovement();
		RotateTowardsMouse();
		AnimatorControllers();
	}

	private void AnimatorControllers()
	{
		float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
		float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

		animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
		animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

		bool playRunAnimation = isRunning && movementDirection.magnitude > 0;

		animator.SetBool("isRunning", playRunAnimation);
	}

	private void RotateTowardsMouse()
	{

		Vector3 lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
		lookingDirection.y = 0f;
		lookingDirection.Normalize();

		Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
		transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);

		//transform.forward = lookingDirection;

	}

	private void ApplyMovement()
	{
		movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
		ApplyGravity();

		if (movementDirection.magnitude > 0)
		{
			characterController.Move(movementDirection * Time.deltaTime * movementSpeed);
		}
	}

	private void ApplyGravity()
	{
		if (!characterController.isGrounded)
		{
			verticalVelocity -= 9.81f * Time.deltaTime;
			movementDirection.y = verticalVelocity;
		}
		else
			verticalVelocity = -0.5f;
	}

	private void AssignInputEvents()
	{
		controls = player.controls;

		controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
		controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

		controls.Character.Run.performed += context =>
		{
			movementSpeed = runSpeed;
			isRunning = true;

		};

		controls.Character.Run.canceled += context =>
		{
			movementSpeed = walkSpeed;
			isRunning = false;
		};
	}
}
