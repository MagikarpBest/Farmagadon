using UnityEngine;
using System.Collections;

public class AudioFade : MonoBehaviour
{
    
    public void FadeOutBGM(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        var unityAudio = AudioService.AudioManager as UnityAudioManager;
        var audioSource = unityAudio.AudioSource;
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            //Mathf.Lerp(a,b,t)    t=0,return a  t=1,return b   time/duration 0->1 gradually
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
