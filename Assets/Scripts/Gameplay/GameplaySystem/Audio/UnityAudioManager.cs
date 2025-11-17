using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mono.Cecil;
public class UnityAudioManager : MonoBehaviour, IAudio
{
    private AudioSource audioSource;
    private List<AudioClip> clipBuffer = new List<AudioClip>();

    public AudioSource AudioSource => audioSource;
    private float initialVolume;

    public void Initiallize()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        float savedVol = PlayerPrefs.GetFloat("mainVolume", 1f);
        audioSource.volume = savedVol;
        initialVolume = savedVol;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayOneShot(AudioClip clip, float volumeScale = 1)
    {
        audioSource.PlayOneShot(clip, volumeScale);
    }

    public void BufferPlayOneShot(AudioClip clip, float volumeScale = 1)
    {
        if (clipBuffer.Contains(clip)) { return; }
        clipBuffer.Add(clip);
        StartCoroutine(PlayBufferedAudioClip(clip, volumeScale));
    }

    public void PlayBGM(AudioClip clip,float duration)
    {
        audioSource.Stop();

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopClip(AudioClip clip)
    {
        audioSource.Stop();
    }

    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    private IEnumerator PlayBufferedAudioClip(AudioClip clip, float volumeScale)
    {
        yield return new WaitForEndOfFrame();
        audioSource.PlayOneShot(clip, volumeScale);
        clipBuffer.Remove(clip);
    }


    public void FadeOutBGM(float duration = 1f)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    public void FadeInBGM(float duration = 1f)
    {
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = AudioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop(); 
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float targetVolume = initialVolume;
        float time = 0f;

        audioSource.volume = 0f;
        audioSource.Play();

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }

        Debug.Log(targetVolume);
        audioSource.volume = targetVolume;
    }

}
