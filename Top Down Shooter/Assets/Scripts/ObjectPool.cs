using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

	[SerializeField] private GameObject bulletPrefab;

	[SerializeField] private int poolSize = 10;

	private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void CreateInitialPool(GameObject prefab)
	{
		poolDictionary[prefab] = new Queue<GameObject>();

		for (int i = 0; i < poolSize; i++)
		{
			CreateObjectAndEnqueue(prefab);
		}
	}

	private void CreateObjectAndEnqueue(GameObject prefab)
	{
		GameObject newObject = Instantiate(prefab, transform);
		newObject.AddComponent<PooledObject>().originalPrefab = prefab;

		newObject.SetActive(false);

		poolDictionary[prefab].Enqueue(newObject);
	}

	public GameObject GetObjectFromPool(GameObject objectPrefab)
	{
		if (!poolDictionary.ContainsKey(objectPrefab))
		{
			CreateInitialPool(objectPrefab);
		}

		if (poolDictionary[objectPrefab].Count == 0)
			CreateObjectAndEnqueue(objectPrefab); //if all objects of this type are in use, create a new one.

		GameObject objectToGet = poolDictionary[objectPrefab].Dequeue();
		objectToGet.SetActive(true);
		objectToGet.transform.parent = null;

		return objectToGet;
	}

	public void ReturnObjectToPoolWithDelay(GameObject objectToReturn, float delay = 0.001f)
	{
		StartCoroutine(DelayReturnObjectToPool(delay, objectToReturn));
	}

	private IEnumerator DelayReturnObjectToPool(float delay, GameObject objectToReturn)
	{
		yield return new WaitForSeconds(delay);

		ReturnObjectToPool(objectToReturn);
	}

	private void ReturnObjectToPool(GameObject objectToReturn)
	{
		GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;

		objectToReturn.SetActive(false);
		objectToReturn.transform.parent = transform;

		poolDictionary[originalPrefab].Enqueue(objectToReturn);
	}
}
