using UnityEngine;

public interface IAudio
{
    void Initiallize();

    void PlayOneShot(AudioClip clip, float volumeScale = 1.0f);

    void PlayBGM(AudioClip clip, float volumeScale = 1.0f);

    void SetPitch(float pitch);

    void SetVolume(float volume);
}
