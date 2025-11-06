using UnityEngine;
using System.Collections;

public class Quit : MonoBehaviour
{

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
