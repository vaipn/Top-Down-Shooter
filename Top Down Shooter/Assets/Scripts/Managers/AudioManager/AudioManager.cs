using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] backgroundMusic;

    [SerializeField] private bool playBackgroundMusic;
    private int backgroundMusicIndex;

	private void Update()
	{
        //if (playBackgroundMusic == false && BackgroundMusicIsPlaying())
        //    StopAllBackgroundMusic();
        //else if (backgroundMusic[backgroundMusicIndex].isPlaying == false)
        //    PlayRandomBackgroundMusic();
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
