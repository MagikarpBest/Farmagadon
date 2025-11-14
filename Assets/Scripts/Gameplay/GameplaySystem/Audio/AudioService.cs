using UnityEngine;

public class AudioService
{
    private static IAudio CURRENT_AUDIO_MANAGER = new NullAudioManager();
    public static IAudio AudioManager => CURRENT_AUDIO_MANAGER;

    public static void SetAudioManager(IAudio audioManager)
    {
        if (audioManager == null)
        {
            CURRENT_AUDIO_MANAGER = new NullAudioManager();
        }
        CURRENT_AUDIO_MANAGER = audioManager;
    }

}
