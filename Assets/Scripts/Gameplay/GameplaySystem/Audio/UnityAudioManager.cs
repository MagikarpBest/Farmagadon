using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class UnityAudioManager : MonoBehaviour, IAudio
{
    private AudioSource audioSource;
    private List<AudioClip> clipBuffer = new List<AudioClip>();

    public AudioSource AudioSource => audioSource;

    public void Initiallize()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
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

    private void Update()
    {
        
    }

    
}
