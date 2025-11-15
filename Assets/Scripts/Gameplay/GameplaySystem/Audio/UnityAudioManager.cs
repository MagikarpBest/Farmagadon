using UnityEngine;

public class UnityAudioManager : MonoBehaviour, IAudio
{
    private AudioSource audioSource;

    public AudioSource AudioSource => audioSource;
    //-------for debugging-----------
    //private void Awake()
    //{
    //    Initiallize();
    //}
    //-------------------------------
    public void Initiallize()
    {
        //-----------for debug----------
        //if (AudioService.AudioManager is NullAudioManager)
        //    AudioService.SetAudioManager(this);
        //------------------------------

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        float savedVol = PlayerPrefs.GetFloat("mainVolume", 1f);
        audioSource.volume = savedVol;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayOneShot(AudioClip clip, float volumeScale = 1)
    {
        float savedVol = PlayerPrefs.GetFloat("mainVolume", 1f);
        audioSource.PlayOneShot(clip, volumeScale * savedVol);
    }

    public void PlayBGM(AudioClip clip, float volumeScale = 1)
    {
        float savedVol = PlayerPrefs.GetFloat("mainVolume", 1f);
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = savedVol;
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

}
