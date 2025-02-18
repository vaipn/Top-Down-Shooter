using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private int bulletPoolSize = 10;

	private Queue<GameObject> bulletPool;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		bulletPool = new Queue<GameObject>();

		CreateInitialPool();
	}

	private void CreateInitialPool()
	{
		for (int i = 0; i < bulletPoolSize; i++)
		{
			CreateBulletAndEnqueue();
		}
	}

	private void CreateBulletAndEnqueue()
	{
		GameObject newBullet = Instantiate(bulletPrefab, transform);
		newBullet.gameObject.SetActive(false);

		bulletPool.Enqueue(newBullet);
	}

	public GameObject GetBulletFromQueue()
	{
		if (bulletPool.Count == 0)
			CreateBulletAndEnqueue();


		GameObject bulletToGet = bulletPool.Dequeue();
		bulletToGet.SetActive(true);
		bulletToGet.transform.parent = null;

		return bulletToGet;
	}

	public void ReturnBulletToQueue(GameObject bullet)
	{
		bullet.SetActive(false);
		bullet.transform.parent = transform;
		bulletPool.Enqueue(bullet);
	}
}
