using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private GameObject bulletImpactFX;
	private Rigidbody rb => GetComponent<Rigidbody>();
	private void OnCollisionEnter(Collision collision)
	{
		CreateImpactFX(collision);
		//rb.constraints = RigidbodyConstraints.FreezeAll;
		ObjectPool.instance.ReturnBulletToQueue(gameObject);
	}

	private void CreateImpactFX(Collision collision)
	{
		if (collision.contacts.Length > 0)
		{
			ContactPoint contact = collision.contacts[0]; // first point of contact

			GameObject newImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

			Destroy(newImpactFX, 1f);
		}
	}
}
