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
}
