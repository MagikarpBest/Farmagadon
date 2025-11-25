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
        //AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat(MasterVolumeFloatKey, masterVolumeSlider.value);
        audioMixer.SetFloat(MasterVolumeFloatKey, (1.0f - masterVolumeSlider.value) * (-80.0f));
        Save();
    }

    public void ChangeSFXVolume()
    {
        //AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat(SFXVolumeFloatKey, sfxVolumeSlider.value);
        audioMixer.SetFloat(SFXVolumeFloatKey, (1.0f - sfxVolumeSlider.value) * (-80.0f));
        Save();
    }

    public void ChangeMusicVolume()
    {
        //AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat(MusicVolumeFloatKey, musicVolumeSlider.value);
        audioMixer.SetFloat(MusicVolumeFloatKey, (1.0f - musicVolumeSlider.value) * (-80.0f));
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
