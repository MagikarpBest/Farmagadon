using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{
    public event Action OnShootAction;
    public event Action OnNextWeapon;
    public event Action OnPreviousWeapon;
    public event Action OnPause;

    private PlayerInput playerInput;

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        playerInput.Player.Pause.performed += Pause_performed;
        playerInput.Player.Shoot.performed += Shoot_performed;
        playerInput.Player.PreviousWeapon.performed += PreviousWeapon_performed;
        playerInput.Player.NextWeapon.performed += NextWeapon_performed;
    }
    private void OnDisable()
    {
        playerInput.Player.Disable();
        playerInput.Player.Pause.performed -= Pause_performed;
        playerInput.Player.Shoot.performed -= Shoot_performed;
        playerInput.Player.PreviousWeapon.performed -= PreviousWeapon_performed;
        playerInput.Player.NextWeapon.performed -= NextWeapon_performed;
    }

    private void PreviousWeapon_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPreviousWeapon?.Invoke();
    }
    private void NextWeapon_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNextWeapon?.Invoke();
    }
    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnShootAction?.Invoke();
        Debug.Log("Space pressed");
    }
    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPause?.Invoke();
        Debug.Log("Esc pressed");
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
