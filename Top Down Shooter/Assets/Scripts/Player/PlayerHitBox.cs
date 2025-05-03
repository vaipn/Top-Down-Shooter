using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : HitBox
{
	private Player player;

	protected override void Awake()
	{
		base.Awake();

		player = GetComponentInParent<Player>();
	}

	public override void TakeDamage(int damage)
	{
		player.health.ReduceHealth(damage);
	}
}
