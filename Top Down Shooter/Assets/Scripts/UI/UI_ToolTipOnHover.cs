using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ToolTipOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	[SerializeField] private GameObject toolTip;

	[Header("Audio")]
	[SerializeField] private AudioSource pointerEnterSFX;
	[SerializeField] private AudioSource pointerDownSFX;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (pointerDownSFX != null)
			pointerDownSFX.Play();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (toolTip != null)
			toolTip.SetActive(true);

		if (pointerEnterSFX != null)
			pointerEnterSFX.Play();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (toolTip != null)
			toolTip.SetActive(false);
	}
}
