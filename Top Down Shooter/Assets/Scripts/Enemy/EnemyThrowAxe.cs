using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrowAxe : MonoBehaviour
{
    public Rigidbody rb;
    public Transform axeVisual;
    public Transform player;
    public float flySpeed;
    public float rotationSpeed;

    public Vector3 direction;


	private void Update()
	{
		axeVisual.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);

        direction = player.position + Vector3.up - transform.position;
        rb.velocity = direction.normalized * flySpeed;

        transform.forward = rb.velocity; // to make sure axe is facing player
	}
}
