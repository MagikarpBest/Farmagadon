using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float switchDelay;


    public void Switch(string scene)
    {
        scene = sceneName;
        AudioService.AudioManager.FadeOutBGM();
        StartCoroutine(SwitchDelay(switchDelay));
    }

    public IEnumerator SwitchDelay(float delay)
    { 
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName); 
    }
}
