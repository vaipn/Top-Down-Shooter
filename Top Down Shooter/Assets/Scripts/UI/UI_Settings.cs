using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
	[Header("Audio mixer")]
	[SerializeField] private AudioMixer audioMixer;
	private float minVolume = -80f;
	private float maxVolume = 10f;

	[Header("SFX Settings")]
	[SerializeField] private Slider sfxSlider;
	[SerializeField] private TextMeshProUGUI sfxSliderText;
	[SerializeField] private string sfxParameter;


	[Header("BGM Settings")]
	[SerializeField] private Slider bgmSlider;
	[SerializeField] private TextMeshProUGUI bgmSliderText;
	[SerializeField] private string bgmParameter;

	[Header("Toggle")]
	[SerializeField] private Toggle friendlyFireToggle;
	public void SFXSliderValue(float value)
	{
		sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
		float audioMixerVolume = Mathf.Lerp(minVolume, maxVolume, value);
		audioMixer.SetFloat(sfxParameter, audioMixerVolume);
	}

	public void BGMSliderValue(float value)
	{
		bgmSliderText.text = Mathf.RoundToInt(value * 100) + "%";
		float audioMixerVolume = Mathf.Lerp(minVolume, maxVolume, value);
		audioMixer.SetFloat(bgmParameter, audioMixerVolume);
	}

	public void OnFriendlyFireToggle()
	{
		bool friendlyFire = GameManager.instance.friendlyFire;
		GameManager.instance.friendlyFire = !friendlyFire;
	}

	public void LoadSettings()
	{
		sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, 0.8f);
		bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, 0.65f);

		int friendlyFireInt = PlayerPrefs.GetInt("FriendlyFire", 0);
		bool friendlyFire = false;

		if (friendlyFireInt == 1)
			friendlyFire = true;

		friendlyFireToggle.isOn = friendlyFire;
	}

	private void OnDisable()
	{
		bool friendlyFire = GameManager.instance.friendlyFire;
		int friendlyFireInt = friendlyFire ? 1 : 0;

		PlayerPrefs.SetInt("FriendlyFire", friendlyFireInt);
		PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
		PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);
	}
}
