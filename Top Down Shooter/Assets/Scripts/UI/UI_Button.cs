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
	public float scaleChangeRate = 5;
	public float scaleAmount = 1.07f;

	private Vector3 defaultScale;
	private Vector3 targetScale;

	private Image buttonImage;
	private TextMeshProUGUI buttonText;

	public bool isQuitButton;

	[Header("Audio")]
	[SerializeField] private AudioSource pointerEnterSFX;
	[SerializeField] private AudioSource pointerDownSFX;
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

		if (pointerEnterSFX != null)
			pointerEnterSFX.Play();

		if (!isQuitButton)
		{
			if (buttonImage != null)
				buttonImage.color = Color.yellow;

			if (buttonText != null)
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

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		ReturnToDefaultLook();

		if (pointerDownSFX != null)
			pointerDownSFX.Play();
	}

	private void ReturnToDefaultLook()
	{
		targetScale = defaultScale;

		if (buttonImage != null)
			buttonImage.color = Color.white;

		if (buttonText != null)
			buttonText.color = Color.white;
	}

	public void AssignAudioSource()
	{
		pointerEnterSFX = GameObject.FindGameObjectWithTag("UI_PointerEnter_Sound").GetComponent<AudioSource>();
		pointerDownSFX = GameObject.FindGameObjectWithTag("UI_PointerDown_Sound").GetComponent<AudioSource>();
	}
}
