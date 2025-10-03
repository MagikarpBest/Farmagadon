using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
public class PlayerFarmInput : MonoBehaviour
{
    [SerializeField] BoxCollider2D cropRadius;
    PlayerInput playerInput;
    private List<GameObject> plants = new List<GameObject>();
    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        playerInput.Player.Shoot.performed += Shoot_performed;
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
    }
    private void OnDestroy()
    {
        playerInput.Player.Disable();
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        for (int i = plants.Count - 1; i >= 0; --i)
        {
            plants[i].GetComponent<plants>().destroySelf();
        }
        plants.Clear();
    }   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!plants.Contains(other.gameObject))
        {
            plants.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (plants.Contains(collision.gameObject))
        {
            plants.Remove(collision.gameObject);
        }
    }
}
