using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLimitation : MonoBehaviour
{
    private ParticleSystem[] lines;
    private BoxCollider zoneCollider;
	private string[] logMessages = new string [4]{ "I wonder what lies there, but who cares, gotta complete the mission",
		"This is uncharted territory, I better not risk the success of the mission",
		"My instinct tells me unimaginable horrors lie ahead",
		"Seems my creator made this area inaccessible to me"};

	private void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;
		zoneCollider = GetComponent<BoxCollider>();
		lines = GetComponentsInChildren<ParticleSystem>();

		ActivateWall(false);
	}

	private void ActivateWall(bool activate)
	{
		foreach (var line in lines)
		{
			if (activate)
				line.Play();
			else
				line.Stop();
		}

		zoneCollider.isTrigger = !activate;
	}

	IEnumerator WallActivationCoroutine()
	{
		ActivateWall(true);

		yield return new WaitForSeconds(1);

		ActivateWall(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		StartCoroutine(WallActivationCoroutine());

		int randomIndex = UnityEngine.Random.Range(0, logMessages.Length);
		Debug.Log(logMessages[randomIndex]);
	}
}
