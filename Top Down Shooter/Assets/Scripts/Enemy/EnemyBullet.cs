using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
	protected override void OnCollisionEnter(Collision collision)
	{
		CreateImpactFX();
		ReturnBulletToPool();

		Player player = collision.gameObject.GetComponentInParent<Player>();
		
		if (player != null)
		{
			Debug.Log("Shot the player");
		}
	}
}
