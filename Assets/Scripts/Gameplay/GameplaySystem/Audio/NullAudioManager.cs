using UnityEngine;
using UnityEngine.Rendering;

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
    public void StopClip(AudioClip clip)
    {
        Debug.Log($"[This is NullAudioManager], you tried to stop {clip.name}");
    }

    public void BufferPlayOneShot(AudioClip clip, float volumeScale = 1)
    {
        Debug.Log($"[This is NullAudioManager], you tried to buffer {clip.name} at volume {volumeScale} but this is not the right manager");
    }

    public void SetPitch(float pitch)
    {
        Debug.Log($"[This is NullAudioManager], you tried to set pitch to {pitch}");
    }

    public void SetVolume(float volume)
    {
        Debug.Log($"[This is NullAudioManager], you tried to set volume to {volume}");
    }

    public void FadeOutBGM(float duration)
    {
        Debug.Log($"[This is NullAudioManager], you tried to fade out bgm");
    }
    public void FadeInBGM(float duration)
    {
        Debug.Log($"[This is NullAudioManager], you tried to fade in bgm");
    }
}
