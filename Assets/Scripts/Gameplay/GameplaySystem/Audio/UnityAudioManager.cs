using UnityEngine;

public class UnityAudioManager : MonoBehaviour, IAudio
{
    private AudioSource audioSource;

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

    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

}
