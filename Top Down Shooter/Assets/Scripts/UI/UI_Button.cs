using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	[Header("Mouse hover settings")]
	public float scaleChangeRate = 1;
	public float scaleAmount = 1.2f;

	private Vector3 defaultScale;
	private Vector3 targetScale;

	private Image buttonImage;
	private TextMeshProUGUI buttonText;

	public bool isQuitButton;
	public virtual void Start()
	{
		defaultScale = transform.localScale;
		targetScale = defaultScale;

		buttonImage = GetComponent<Button>().image;
		buttonText = GetComponentInChildren<TextMeshProUGUI>();
	}

	public virtual void Update()
	{
		if (Mathf.Abs(transform.localScale.x - targetScale.x) > 0.01f)
		{
			float scaleValue = Mathf.Lerp(transform.localScale.x, targetScale.x, Time.deltaTime * scaleChangeRate);

			transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		targetScale = defaultScale * scaleAmount;

		if (!isQuitButton)
		{
			buttonImage.color = Color.yellow;
			buttonText.color = Color.yellow;
		}
		else
		{
			buttonImage.color = Color.red;
			buttonText.color = Color.red;
		}
		
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		ReturnToDefaultLook();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		ReturnToDefaultLook();
	}

	private void ReturnToDefaultLook()
	{
		targetScale = defaultScale;

		buttonImage.color = Color.white;
		buttonText.color = Color.white;
	}

}
