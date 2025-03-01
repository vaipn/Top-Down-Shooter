using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public List<Interactable> interactables;

    public Interactable closestInteractable;
    public float closestInteractableDistance;
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
}
