using UnityEngine;

namespace Farm
{
    public class PlayerFarmMovement : MonoBehaviour
    {
        PlayerInput playerInput;
        [SerializeField] GameObject player;
        [SerializeField] GridController gridController;
        Vector3Int playerPos = new();


        private void Awake()
        {
            playerInput = new PlayerInput();
            playerPos = gridController.PlayerStartPos;
            player.transform.position = gridController.TileMap.GetCellCenterWorld(playerPos);
            playerInput.Player.Enable();
            playerInput.Player.Farm.performed += Farm_performed;

        }

        private void Farm_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Vector2 inputVector = playerInput.Player.Farm.ReadValue<Vector2>().normalized;
            int x = (int)inputVector.x;
            int y = (int)inputVector.y;
            Vector3Int moveDir = new Vector3Int(x, y, 0) + playerPos;

            int xBoundary = (int)Mathf.Clamp(moveDir.x, gridController.TileMap.cellBounds.min.x, gridController.TileMap.cellBounds.max.x - 1);
            int yBoundary = (int)Mathf.Clamp(moveDir.y, gridController.TileMap.cellBounds.min.y, gridController.TileMap.cellBounds.max.y - 1);

            moveDir =  new Vector3Int(xBoundary, yBoundary, 0);


            playerPos = moveDir;

            player.transform.position = gridController.Grid.GetCellCenterWorld(playerPos);


        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
