using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrowAxe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
	[SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;


    private Vector3 direction;
    private Transform player;
    private float flySpeed;
    private float rotationSpeed = 1600;
    private float timer = 1;

    private int damage;

    public void AxeSetup(float flySpeed, Transform player, float timer, int damage)
    {
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
        this.damage = damage;
    }

	private void Update()
	{
		axeVisual.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position;

        transform.forward = rb.velocity; // to make sure axe is facing player
	}

	private void FixedUpdate()
	{
        rb.velocity = direction.normalized * flySpeed;
	}

	private void OnCollisionEnter(Collision collision)
	{
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(damage);

		GameObject newFx = ObjectPool.instance.GetObjectFromPool(impactFx, transform);

		ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject);
		ObjectPool.instance.ReturnObjectToPoolWithDelay(newFx, 1);
	}
}
