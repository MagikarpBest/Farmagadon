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
<<<<<<< HEAD
    public void PlayBGM(AudioClip clip, float volumeScale = 1)
    {
        Debug.Log($"[This is NullAudioManager], you tried to play {clip.name} at volume {volumeScale} but this is not the right manager");
    }
    public void StopClip(AudioClip clip)
    {
        Debug.Log($"[This is NullAudioManager], you tried to stop {clip.name}");
    }
=======
>>>>>>> 14/11-chris

    public void SetPitch(float pitch)
    {
        Debug.Log($"[This is NullAudioManager], you tried to set pitch to {pitch}");
    }

    public void SetVolume(float volume)
    {
        Debug.Log($"[This is NullAudioManager], you tried to set volume to {volume}");
    }

}
