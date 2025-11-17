using UnityEngine;
using UnityEngine.UI;
public class VolumeManager : MonoBehaviour 
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioClip mainMenuBGM;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("mainVolume"))
        {
            PlayerPrefs.SetFloat("mainVolume", 1);
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
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("mainVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("mainVolume",volumeSlider.value);
    }
}
