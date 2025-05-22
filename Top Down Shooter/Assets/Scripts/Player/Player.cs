using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Transform playerBody;

    public PlayerControls controls { get; private set; } // read-only
	public PlayerAim aim { get; private set; } // read-only
	public PlayerMovement movement { get; private set; } // read-only
	public PlayerWeaponController weaponController { get; private set; } // read-only
	public PlayerWeaponVisuals weaponVisuals { get; private set; }
	public PlayerInteraction interaction { get; private set; }
	public PlayerHealth health { get; private set; }
	public Ragdoll ragdoll { get; private set; }
	public Animator anim { get; private set; }

	public bool controlsEnabled { get; private set; }
	private void Awake()
	{
		controls = new PlayerControls();


		aim = GetComponent<PlayerAim>();
		movement = GetComponent<PlayerMovement>();
		weaponController = GetComponent<PlayerWeaponController>();
		weaponVisuals = GetComponent<PlayerWeaponVisuals>();
		interaction = GetComponent<PlayerInteraction>();
		health = GetComponent<PlayerHealth>();
		ragdoll = GetComponent<Ragdoll>();
		anim = GetComponentInChildren<Animator>();
	}

	private void OnEnable()
	{
		controls.Enable();
		controls.Character.UIPause.performed += ctx => UI.instance.PauseSwitch();
	}

	private void OnDisable()
	{
		controls.Disable();
	}

	public void SetControlsEnabledTo(bool enabled) => controlsEnabled = enabled;
}
