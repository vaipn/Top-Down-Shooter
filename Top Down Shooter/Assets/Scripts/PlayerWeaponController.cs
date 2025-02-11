using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
	private Animator animator;

	private void Start()
	{
		player = GetComponent<Player>();
		animator = GetComponentInChildren<Animator>();

		player.controls.Character.Fire.performed += context => Shoot();
	}

	private void Shoot()
	{
		animator.SetTrigger("Fire");
	}
}
