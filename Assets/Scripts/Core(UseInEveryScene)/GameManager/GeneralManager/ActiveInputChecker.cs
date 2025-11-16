using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class ActiveInputChecker : MonoBehaviour
{

    public static INPUT_DEVICE_TYPE currentInputType = INPUT_DEVICE_TYPE.Keyboard;
    private PlayerInput playerInput;
    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        print("DDDDDDDDDDDDD");
    }

    private void Start()
    {

        //print("AM I RUNNING");
        ////
        print("AM I RUNNING");
        StartCoroutine(Test());
        print("AM I RUNNING");


    }
    private IEnumerator Test()
    {
        while(true)
        {
            print(playerInput.bindingMask == InputBinding.MaskByGroup("Player"));
            
            yield return null;
            break;
        }
        
    }

}
