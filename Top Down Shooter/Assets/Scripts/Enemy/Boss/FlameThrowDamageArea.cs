using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowDamageArea : MonoBehaviour
{
    private EnemyBoss enemy;
	private float damageCooldown;
	private float lastTimeDamaged;
	private void Awake()
	{
		enemy = GetComponentInParent<EnemyBoss>();
		damageCooldown = enemy.flameDamageCooldown;
	}

	private void OnTriggerStay(Collider other)
	{
		if (!enemy.flameThrowActive)
			return;

		if (Time.time - lastTimeDamaged < damageCooldown)
			return;

		IDamagable damagable = other.GetComponent<IDamagable>();

        if (damagable != null)
        {
			damagable.TakeDamage();
            lastTimeDamaged = Time.time;
			damageCooldown = enemy.flameDamageCooldown; // for easier testing, it updates with enemy boss variable every time enemy is damaged
        }
	}
}
