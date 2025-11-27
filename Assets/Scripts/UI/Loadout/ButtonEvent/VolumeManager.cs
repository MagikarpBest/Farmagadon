using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeManager : MonoBehaviour 
{
    public const string MasterVolumeFloatKey = "MASTERVolume";
    public const string SFXVolumeFloatKey = "SFXVolume";
    public const string MusicVolumeFloatKey = "MUSICVolume";
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip mainMenuBGM;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(MasterVolumeFloatKey))
        {
            PlayerPrefs.SetFloat(MasterVolumeFloatKey, 1.0f);
            PlayerPrefs.SetFloat(SFXVolumeFloatKey, 1.0f);
            PlayerPrefs.SetFloat(MusicVolumeFloatKey, 1.0f);
            Load();
        }
        else
        {
            Load();
        }
        UnityAudioManager unityAudioManagerPrefab = Resources.Load<UnityAudioManager>("UnityAudioManager");
        UnityAudioManager unityAudioManagerInstance = GameObject.Instantiate(unityAudioManagerPrefab);
        unityAudioManagerInstance.Initiallize();
        unityAudioManagerInstance.name = "AudioManager";
        AudioService.SetAudioManager(unityAudioManagerInstance);
        AudioService.AudioManager.PlayBGM(mainMenuBGM);
    }
    public void ChangeMasterVolume()
    {
        float slider = masterVolumeSlider.value;
        float dB;

        if (slider <= 0f)
        {
            // mute
            dB = -80f;
        }
        else
        {
            // Logarithmic audio curve (smooth + not silent in middle)
            dB = Mathf.Log10(slider) * 20f;
        }

        audioMixer.SetFloat(MasterVolumeFloatKey, dB);

        PlayerPrefs.SetFloat(MasterVolumeFloatKey, slider);
        Save();
    }

    public void ChangeSFXVolume()
    {
        float slider = sfxVolumeSlider.value;
        float dB;

        if (slider <= 0f)
        {
            // mute
            dB = -80f; 
        }
        else
        {
            // Logarithmic audio curve (smooth + not silent in middle)
            dB = Mathf.Log10(slider) * 20f; 
        }

        audioMixer.SetFloat(SFXVolumeFloatKey, dB);

        PlayerPrefs.SetFloat(SFXVolumeFloatKey, slider);
        Save();
    }

    public void ChangeMusicVolume()
    {
        float slider = musicVolumeSlider.value;
        float dB;

        if (slider <= 0f)
        {
            // Mute
            dB = -80f;
        }
        else
        {
            // Logarithmic audio curve (smooth + not silent in middle)
            dB = Mathf.Log10(slider) * 20f;
        }

        audioMixer.SetFloat(MusicVolumeFloatKey, dB);

        PlayerPrefs.SetFloat(MusicVolumeFloatKey, slider);
        Save();
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey(MasterVolumeFloatKey)) return;
        masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolumeFloatKey);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFXVolumeFloatKey);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolumeFloatKey);
    }

    private void Save()
    {
        //PlayerPrefs.SetFloat("mainVolume",volumeSlider.value);
    }
}
