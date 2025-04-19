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

    public void AxeSetup(float flySpeed, Transform player, float timer)
    {
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

	private void Update()
	{
		axeVisual.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position;


        rb.velocity = direction.normalized * flySpeed;

        transform.forward = rb.velocity; // to make sure axe is facing player
	}

	private void OnTriggerEnter(Collider other)
	{
		Bullet bullet = other.GetComponent<Bullet>();
        Player player = other.GetComponent<Player>();

        if (bullet != null || player != null)
        {
            GameObject newFx = ObjectPool.instance.GetObjectFromPool(impactFx, transform);
            //newFx.transform.position = transform.position;

            ObjectPool.instance.ReturnObjectToPoolWithDelay(gameObject);
            ObjectPool.instance.ReturnObjectToPoolWithDelay(newFx, 1);
        }
	}
}
