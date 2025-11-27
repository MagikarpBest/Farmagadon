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
        Debug.LogWarning($"[This is NullAudioManager], you tried to play {clip.name} at volume {volumeScale} but this is not the right manager");
    }
    public void PlayBGM(AudioClip clip, float volumeScale = 1)
    {
        Debug.LogWarning($"[This is NullAudioManager], you tried to play {clip.name} at volume {volumeScale} but this is not the right manager");
    }
    public void StopClip()
    {
        Debug.Log($"[This is NullAudioManager], you tried to stop a track");
    }

    public void BufferPlayOneShot(AudioClip clip, float volumeScale = 1)
    {
        Debug.LogWarning($"[This is NullAudioManager], you tried to buffer {clip.name} at volume {volumeScale} but this is not the right manager");
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
