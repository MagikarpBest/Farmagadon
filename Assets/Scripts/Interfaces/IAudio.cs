using UnityEngine;

public interface IAudio
{
    void Initiallize();

    void PlayOneShot(AudioClip clip, float volumeScale = 1.0f);

<<<<<<< HEAD
    void PlayBGM(AudioClip clip, float volumeScale = 1.0f);

    void StopClip(AudioClip clip);

=======
>>>>>>> 14/11-chris
    void SetPitch(float pitch);

    void SetVolume(float volume);
}
