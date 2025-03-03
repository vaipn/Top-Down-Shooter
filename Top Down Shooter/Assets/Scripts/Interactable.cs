using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
	protected MeshRenderer meshRenderer;
	protected Material defaultMaterial;
	[SerializeField] Material highlightMaterial;

	private void Start()
	{
		if (meshRenderer == null)
			meshRenderer = GetComponentInChildren<MeshRenderer>();

		defaultMaterial = meshRenderer.sharedMaterial;
	}

	protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
	{
		meshRenderer = newMesh;
		defaultMaterial = newMesh.sharedMaterial;
	}

	public void HighlightActive(bool active)
	{
		if (active)
			meshRenderer.material = highlightMaterial;
		else
			meshRenderer.material = defaultMaterial;
	}

	public virtual void Interaction()
	{
		Debug.Log("Interacted with " + gameObject.name);
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

		if (playerInteraction == null )
			return;
		
		
		playerInteraction.interactables.Add(this);
		playerInteraction.UpdateClosestInteractable();
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

		if (playerInteraction == null)
			return;

	
		playerInteraction.interactables.Remove(this);
		playerInteraction.UpdateClosestInteractable();
	}
}
