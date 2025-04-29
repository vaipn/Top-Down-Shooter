using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthController
{
	private Player player;
	public bool isDead {  get; private set; }
	protected override void Awake()
	{
		base.Awake();

		player = GetComponent<Player>();
	}
	public override void ReduceHealth()
	{
		base.ReduceHealth();

		if (ShouldDie())
			Die();

	}

	private void Die()
	{
		isDead = true;
		player.anim.enabled = false;
		player.ragdoll.RagdollActive(true);
	}
}
