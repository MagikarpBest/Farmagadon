using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] GameInput gameInput;

    [Header("Movement Bounds")]
    [SerializeField] float minX = 0f;
    [SerializeField] float maxX = 0f;
    private void Update()
    {
        HandleMovement();
    }

    // Function that handle movement logics, i just separate incase theres extra extend to movement in future
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, 0f);

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        ClampPosition();
    }

    // Function to check is player out of bounds
    private void ClampPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        transform.position = position;
    }
}
