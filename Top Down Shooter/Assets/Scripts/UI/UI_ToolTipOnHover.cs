using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ToolTipOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private GameObject toolTip;
	public void OnPointerEnter(PointerEventData eventData)
	{
		toolTip?.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		toolTip?.SetActive(false);
	}
}
