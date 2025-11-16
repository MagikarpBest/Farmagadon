using UnityEngine;

public interface IAudio
{
    void Initiallize();

    void PlayOneShot(AudioClip clip, float volumeScale = 1.0f);

    void BufferPlayOneShot(AudioClip clip, float volumeScale = 1.0f);

    void SetPitch(float pitch);

    void SetVolume(float volume);
}
