using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] backgroundMusic;

    [SerializeField] private bool playBackgroundMusic;
    private int backgroundMusicIndex;

	private void Awake()
	{
		instance = this;
	}
	private void Update()
	{
        if (playBackgroundMusic == false && BackgroundMusicIsPlaying())
            StopAllBackgroundMusic();
        

        if (playBackgroundMusic && backgroundMusic[backgroundMusicIndex].isPlaying == false)
            PlayRandomBackgroundMusic();
    }

    public void PlaySFX(AudioSource sfx, bool randomPitch = false, float minPitch = 0.85f, float maxPitch = 1.1f)
    {
        if (sfx == null)
            return;

        float pitch = Random.Range(minPitch, maxPitch);

        sfx.pitch = pitch;
        sfx.Play();
    }

    public void PlaySFXWithDelayAndFade(AudioSource audioSource, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        StartCoroutine(SFXDelayAndFade(audioSource, play, targetVolume, delay, fadeDuration));
    }
	public void PlayBackgroundMusic(int index)
    {
        StopAllBackgroundMusic();

        backgroundMusicIndex = index;
        backgroundMusic[backgroundMusicIndex].Play();
    }

    public void StopAllBackgroundMusic()
    {
        for (int i = 0; i < backgroundMusic.Length; i++)
        {
            backgroundMusic[i].Stop();
        }
    }

    [ContextMenu("Play Random background music")]
    public void PlayRandomBackgroundMusic()
    {
        StopAllBackgroundMusic();

        backgroundMusicIndex = Random.Range(0, backgroundMusic.Length);

        PlayBackgroundMusic(backgroundMusicIndex);
    }

    private bool BackgroundMusicIsPlaying()
    {
        for (int i = 0; i < backgroundMusic.Length; i++)
        {
            if (backgroundMusic[i].isPlaying)
                return true;
        }
        return false;
    }

    private IEnumerator SFXDelayAndFade(AudioSource audioSource, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        yield return new WaitForSeconds(delay);

        float startVolume = play ? 0 : audioSource.volume;
        float endVolume = play ? targetVolume : 0;
        float elapsed = 0;

        if (play)
        {
            audioSource.volume = 0;
            audioSource.Play();
        }

        // Fade in/out over the duration
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = endVolume; // snap volume to endVolume

        if (!play)
            audioSource.Stop();
    }
}
