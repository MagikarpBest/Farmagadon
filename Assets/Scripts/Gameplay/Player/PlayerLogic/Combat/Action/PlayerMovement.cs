using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private PlayerVisualHandler playerVisualHandler;
    [SerializeField] private Animator animator;


    [Header("Movement Bounds")]
    [SerializeField] float minX = 0f;
    [SerializeField] float maxX = 0f;

    public bool canMove = true;
    private void Update()
    {
        if (canMove)
        {
            HandleMovement();
        }
    }

    // Function that handle movement logics, i just separate incase theres extra extend to movement in future
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Get horizontal direction
        float horizontalInput = inputVector.x;

        
        Vector3 moveDir = new Vector3(inputVector.x, 0.0f, 0f);
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // Detect direction (-1 = left, 1 = right, 0 = idle)
        int animationMoveDirection = 0;
        if (horizontalInput >= 0.01f) animationMoveDirection = 1;
        else if (horizontalInput < -0.01f) animationMoveDirection = -1;

        playerVisualHandler.PlayMoveAnimation(animationMoveDirection);

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
