using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] float SceanChangeTime = 3f;
    [SerializeField] private AudioSource audioBGMSource;
    [SerializeField] private AudioSource audioCutChangeSource;
    [SerializeField] private AudioSource audioStageClearSource;
    // Start is called before the first frame update

    public void AudioInit()
    {
<<<<<<< Updated upstream
        audioBGMSource.volume = 0.1f;
        audioCutChangeSource.volume = 0.05f;
        audioStageClearSource.volume = 0.05f;
        StartCoroutine(fadeAudio(true));
=======
        SetBgmVolume(0.1f);
        cutChangeAudioSource.volume = 0.05f;
        stageClearAudioSource.volume = 0.05f;
        StartCoroutine(FadeAudio(true));
>>>>>>> Stashed changes
    }


    public void PlayStageClaerAudio()
    {
        audioStageClearSource.Play();
        StartCoroutine("fadeAudio", false);
    }

    public void PlayCutChangeAudio()
    {
        audioCutChangeSource.Play();
    }

    public void FadingAudio(bool fade)
    {
        StartCoroutine("fadeAudio", fade);
    }


    IEnumerator fadeAudio(bool fade)
    {
        float dirTime = Time.time + SceanChangeTime;
        while (Time.time < dirTime)
        {
            audioBGMSource.volume += (fade) ? 0.01f : -0.018f;
            if (fade)
            {
                if (audioBGMSource.volume > 0.3f)
                    StopCoroutine("fadeAudio");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
