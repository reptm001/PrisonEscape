using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip ambientClip;
    private AudioClip tenseClip;
    private int audioClipPos = 0;

    private bool transitioning = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ambientClip = SoundManager.GetAudioClip(SoundManager.Sound.BGAmbient);
        tenseClip = SoundManager.GetAudioClip(SoundManager.Sound.BGTense);
        audioSource.time = 0f;
        audioSource.clip = ambientClip;
        audioSource.Play();
    }

    public void PlayAmbient()
    {
        if (audioSource.clip != ambientClip)
            if (!transitioning)
            {
                transitioning = true;
                StartCoroutine(FadeOutIn(ambientClip, true));
            }
    }

    public void PlayTense()
    {
        if (audioSource.clip != tenseClip)
            if (!transitioning)
            {
                transitioning = true;
                StartCoroutine(FadeOutIn(tenseClip, false));
            }
    }

    IEnumerator FadeOutIn(AudioClip audio, bool ambient)
    {
        float duration = 0.5f;
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }

        if (ambient)
        {
            audioSource.clip = audio;
            audioSource.timeSamples = audioClipPos;
        } 
        else
        {
            audioClipPos = audioSource.timeSamples;
            audioSource.clip = audio;
            audioSource.timeSamples = 0;
        }
        audioSource.Play();

        currentTime = 0;
        start = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 1, currentTime / duration);
            yield return null;
        }
        transitioning = false;
        yield break;
    }
}
