using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
	private EnemyMelee enemy;
	[SerializeField] private int durability;

	private void Awake()
	{
		enemy = GetComponentInParent<EnemyMelee>();
	}

	public void ReduceDurability()
	{
		durability--;

		if (durability <= 0)
		{
			enemy.anim.SetFloat("ChaseIndex", 0); // Enables default chase animation
			Destroy(gameObject);
		}
	}
}
