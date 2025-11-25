using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;

public class UnityAudioManager : MonoBehaviour, IAudio
{
    private const string SFXFloatKey = VolumeManager.SFXVolumeFloatKey;
    private const string MUSICFloatKey = VolumeManager.MusicVolumeFloatKey;
    [SerializeField] private AudioSource SFXAudioSource;
    [SerializeField] private AudioSource MUSICAudioSource;

    private List<AudioClip> clipBuffer = new List<AudioClip>();

    public AudioSource AudioSource => SFXAudioSource;
    public AudioSource AudioSFX => SFXAudioSource;
    public AudioSource AudioMUSIC => MUSICAudioSource;


    public void Initiallize()
    {
        SFXAudioSource.playOnAwake = false;
        MUSICAudioSource.playOnAwake = false;
        MUSICAudioSource.loop = true;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayOneShot(AudioClip clip, float volumeScale = 1.0f)
    {
        SFXAudioSource.PlayOneShot(clip, volumeScale);
    }

    public void BufferPlayOneShot(AudioClip clip, float volumeScale = 1.0f)
    {
        if (clipBuffer.Contains(clip)) { return; }
        clipBuffer.Add(clip);
        StartCoroutine(PlayBufferedAudioClip(clip, volumeScale));
    }

    public void PlayBGM(AudioClip clip,float duration)
    {
        MUSICAudioSource.Stop();
        MUSICAudioSource.clip = clip;
        MUSICAudioSource.Play();
    }

    public void StopClip()
    {
        MUSICAudioSource.Stop();
    }


    private IEnumerator PlayBufferedAudioClip(AudioClip clip, float volumeScale)
    {
        yield return new WaitForEndOfFrame();
        SFXAudioSource.PlayOneShot(clip, volumeScale);
        clipBuffer.Remove(clip);
    }


    public void FadeOutBGM(float duration = 1.0f)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    public void FadeInBGM(float duration = 1.0f)
    {
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        
        float startVolume = MUSICAudioSource.volume;
        float time = 0.0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            MUSICAudioSource.volume = Mathf.Lerp(startVolume, 0.0f, time / duration);
            yield return null;
        }
        MUSICAudioSource.volume = 0f;
        MUSICAudioSource.Stop(); 
        
        yield return null;
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        
        float targetVolume = MUSICAudioSource.volume;
        float time = 0.0f;

        MUSICAudioSource.volume = 0f;
        MUSICAudioSource.Play();

        while (time < duration)
        {
            time += Time.deltaTime;
            MUSICAudioSource.volume = Mathf.Lerp(0.0f, targetVolume, time / duration);
            yield return null;
        }

        Debug.Log(targetVolume);
        MUSICAudioSource.volume = targetVolume;
        
        yield return null;
    }

}
