using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IDamagable
{
	private EnemyMelee enemy;
	[SerializeField] private int durability;

	private void Awake()
	{
		enemy = GetComponentInParent<EnemyMelee>();

		durability = enemy.shieldDurability;
	}

	public void ReduceDurability()
	{
		durability--;

		if (durability <= 0)
		{
			enemy.anim.SetFloat("ChaseIndex", 0); // Enables default chase animation
			gameObject.SetActive(false);
		}
	}

	public void TakeDamage()
	{
		ReduceDurability();
	}
}
