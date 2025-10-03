using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] GameInput gameInput;
    private void Update()
    {
        HandleMovement();
    }

    //function that handle movement logics, i just separate incase theres extra extend to movement in future
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, 0f);

        transform.position += moveDir * moveSpeed * Time.deltaTime;

    }
}
