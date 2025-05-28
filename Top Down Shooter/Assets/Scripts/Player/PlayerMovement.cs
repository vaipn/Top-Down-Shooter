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

	private AudioSource walkSFX;
	private AudioSource runSFX;

	private bool canPlayFootsteps;
	public Vector2 moveInput { get; private set; }
	private void Start()
	{
		player = GetComponent<Player>();

		characterController = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();

		walkSFX = player.soundFX.walkSFX;
		runSFX = player.soundFX.runSFX;

		Invoke(nameof(AllowFootstepsSFX), 1f); // so footsteps don't sound immediately player is setup and level has not been shown.

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
			PlayFootstepsSFX();

			characterController.Move(movementDirection * Time.deltaTime * movementSpeed);
		}
	}

	private void PlayFootstepsSFX()
	{
		if (!canPlayFootsteps)
			return;

		if (isRunning)
		{
			if (runSFX.isPlaying == false)
				runSFX.Play();
		}
		else
		{
			if (walkSFX.isPlaying == false)
				walkSFX.Play();
		}
	}
	private void StopFootstepsSFX()
	{
		walkSFX.Stop();
		runSFX.Stop();
	}

	private void AllowFootstepsSFX() => canPlayFootsteps = true;
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
		controls.Character.Movement.canceled += context =>
		{
			StopFootstepsSFX();
			moveInput = Vector2.zero;
		};

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
