using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ComicPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image[] comicImages;
    [SerializeField] private int imageIndex;
    [SerializeField] private GameObject playButton;

    private Image comicPanelImage;
    [SerializeField] private bool comicShowOver;

	private void Start()
	{
		comicPanelImage = GetComponent<Image>();
		ShowNextImage();
	}

	private void ShowNextImage()
	{
		if (comicShowOver)
			return;

		StartCoroutine(ChangeImageAlpha(1, 1.5f, ShowNextImage));
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		ShowNextImageOnClick();
	}

	private void ShowNextImageOnClick()
	{
		comicImages[imageIndex].color = Color.white;
		imageIndex++;

		if (imageIndex >= comicImages.Length)
			EnablePlayButton();

		if (comicShowOver)
			return;

		ShowNextImage();
	}

	private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action onComplete)
	{
		float time = 0;
		Color currentColor = comicImages[imageIndex].color;
		float startAlpha = currentColor.a;

		while (time < duration)
		{
			time += Time.deltaTime;
			float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

			comicImages[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
			yield return null; // so the loop runs once per frame
		}

		comicImages[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

		imageIndex++;

		if (imageIndex >= comicImages.Length)
		{
			EnablePlayButton();
		}

		// call the completion method if it exists
		onComplete?.Invoke();
	}

	private void EnablePlayButton()
	{
		StopAllCoroutines();
		comicShowOver = true;
		playButton.SetActive(true);
		comicPanelImage.raycastTarget = false;
	}
}
