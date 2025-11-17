using UnityEngine;

public interface IAudio
{
    void Initiallize();

    void PlayOneShot(AudioClip clip, float volumeScale = 1.0f);

    void BufferPlayOneShot(AudioClip clip, float volumeScale = 1.0f);

    void PlayBGM(AudioClip clip, float volumeScale = 1.0f);

    void StopClip(AudioClip clip);

    void SetPitch(float pitch);

    void SetVolume(float volume);
}
