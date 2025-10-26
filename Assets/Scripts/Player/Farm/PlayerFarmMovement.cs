using UnityEngine;

namespace Farm
{
    public class PlayerFarmMovement : MonoBehaviour
    {
        PlayerInput playerInput;
        [SerializeField] GameObject player;
        [SerializeField] GridController gridController;
        Vector3Int playerPos = new();
        private bool movementDone = false;
        private Vector3 playerInitialPos = Vector3.zero;
        private Vector3 playerFinalPos = Vector3.zero;
        private float startTime = 0;
        private float t = 0;
        private void Awake()
        {
            playerInput = new PlayerInput();
            playerPos = gridController.PlayerStartPos;
            player.transform.position = gridController.TileMap.GetCellCenterWorld(playerPos);
            playerInput.Player.Enable();
            playerInput.Player.Farm.performed += OnFarmMovement;
            startTime = Time.time;
        }

        private void OnDisable()
        {
            playerInput.Player.Farm.performed -= OnFarmMovement;
            playerInput.Player.Disable();
        }

        private void OnFarmMovement(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (movementDone) { return; }
            Vector2 inputVector = playerInput.Player.Farm.ReadValue<Vector2>().normalized;
            int x = (int)inputVector.x;
            int y = (int)inputVector.y;
            playerInitialPos = gridController.Grid.GetCellCenterWorld(playerPos);
            Vector3Int moveDir = new Vector3Int(x, y, 0) + playerPos;

            int xBoundary = (int)Mathf.Clamp(moveDir.x, gridController.TileMap.cellBounds.min.x, gridController.TileMap.cellBounds.max.x - 1);
            int yBoundary = (int)Mathf.Clamp(moveDir.y, gridController.TileMap.cellBounds.min.y, gridController.TileMap.cellBounds.max.y - 1);

            moveDir =  new Vector3Int(xBoundary, yBoundary, 0);

            playerFinalPos = gridController.Grid.GetCellCenterWorld(moveDir);

            playerPos = moveDir;
            movementDone = true;
        }

        

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (movementDone)
            {
                if (Time.time - startTime >= 0.05f)
                {
                    t += 0.2f;
                    startTime = Time.time;
                    player.transform.position = Vector3.Lerp(playerInitialPos, playerFinalPos, t);
                }
                if (t>= 1.0f)
                {
                    t -= 1.0f;
                    movementDone = false;
                }
            }
        }
    }
}
