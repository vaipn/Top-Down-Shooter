using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();

    public Interactable closestInteractable;
    public float closestInteractableDistance;

	private void Start()
	{
		Player player = GetComponent<Player>();

        player.controls.Character.Interaction.performed += context => InteractWithClosest();
	}

	private void InteractWithClosest()
    {
        closestInteractable?.Interaction();
        interactables.Remove(closestInteractable);

        UpdateClosestInteractable();
    }

    public void UpdateClosestInteractable()
    {
        closestInteractable?.HighlightActive(false);

        closestInteractable = null;

        closestInteractableDistance = float.MaxValue;

        foreach (Interactable interactable in interactables)
        {
            float interactableDistance = Vector3.Distance(transform.position, interactable.transform.position);

            if (interactableDistance < closestInteractableDistance)
            {
                closestInteractableDistance = interactableDistance;
                closestInteractable = interactable;
            }
        }

        closestInteractable?.HighlightActive(true);


    }

    public List<Interactable> GetInteractables() => interactables;
}
