using UnityEngine;

public class NullAudioManager : IAudio
{
    
    public void Initiallize()
    {
        Debug.Log($"[This is NullAudioManager]");
    }

    public void PlayOneShot(AudioClip clip, float volumeScale = 1)
    {
        Debug.Log($"[This is NullAudioManager], you tried to play {clip.name} at volume {volumeScale} but this is not the right manager");
    }
    public void PlayBGM(AudioClip clip, float volumeScale = 1)
    {
        Debug.Log($"[This is NullAudioManager], you tried to play {clip.name} at volume {volumeScale} but this is not the right manager");
    }

    public void SetPitch(float pitch)
    {
        Debug.Log($"[This is NullAudioManager], you tried to set pitch to {pitch}");
    }

    public void SetVolume(float volume)
    {
        Debug.Log($"[This is NullAudioManager], you tried to set volume to {volume}");
    }

}
