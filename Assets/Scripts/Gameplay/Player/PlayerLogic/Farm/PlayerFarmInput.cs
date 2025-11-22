using Farm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class PlayerFarmInput : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PolygonCollider2D cropRadius;
    [SerializeField] private GridController gridController;
    [SerializeField] private PlayerFarmAnimator playerFarmAnimator;
    [SerializeField] private float farmActionCooldown = 1.0f;

    public PolygonCollider2D CropRadius { get { return cropRadius; } }
    
    private List<GameObject> plants = new List<GameObject>();

    private bool movementDone = false;
    private bool farmDone = false;

    private float faCD = 0.0f;
    private float moveCD = 0.0f;

    private PlayerInput playerInput;
    private Vector3Int playerPos = Vector3Int.zero;
    private Vector3 playerInitialPos = Vector3.zero;
    private Vector3 playerFinalPos = Vector3.zero;

    public Vector3Int PlayerPos { get { return playerPos; } }

    public delegate void MovementEvent(Vector2 movementVector);
    public MovementEvent OnMovementEvent;
    public delegate void FarmInputPerformed(bool farming);
    public FarmInputPerformed OnFarmInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        player.position = gridController.TileMap.GetCellCenterWorld(playerPos);
        playerInput.Player.Shoot.performed += OnFarmPerformed;
        //playerInput.Player.Move.performed += OnFarmMovement;
    }

    private void OnDestroy()
    {
        playerInput.Player.Disable();
        playerInput.Player.Shoot.performed -= OnFarmPerformed;
        //playerInput.Player.Move.performed -= OnFarmMovement;
        StopAllCoroutines();

    }

    private void OnEnable()
    {
        playerFarmAnimator.AnimReachedGroundPound += GrabPlants;
        
    }

    private void OnDisable()
    {
        playerFarmAnimator.AnimReachedGroundPound -= GrabPlants;
    }
    private void OnFarmMovement(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!FarmController.GetCycleStarted) { return; }
        if (playerFarmAnimator.PlayerAnimator.GetBool("digBool") == true) { return; }
        if (movementDone) { return; }
        
        movementDone = true;
        Vector2 dir = playerInput.Player.Move.ReadValue<Vector2>().normalized;
        
        int x = (int)dir.x;
        int y = (int)dir.y;
        playerInitialPos = gridController.Grid.GetCellCenterWorld(playerPos);
        Vector3Int moveDir = new Vector3Int(x, y, 0) + playerPos;

        int xBoundary = (int)Mathf.Clamp(moveDir.x, gridController.TileMap.cellBounds.min.x, gridController.TileMap.cellBounds.max.x - 1);
        int yBoundary = (int)Mathf.Clamp(moveDir.y, gridController.TileMap.cellBounds.min.y, gridController.TileMap.cellBounds.max.y - 1);

        moveDir = new Vector3Int(xBoundary, yBoundary, 0);

        playerFinalPos = gridController.Grid.GetCellCenterWorld(moveDir);

        playerPos = moveDir;
        OnMovementEvent?.Invoke(dir);
        player.DOMove(playerFinalPos, 0.833f).SetEase(Ease.InOutQuint);
        StartCoroutine(MovementInternalCooldown());
    }

    private void Update()
    {
        if (playerInput.Player.Move.IsPressed())
        {
            if (!FarmController.GetCycleStarted) { return; }
            if (playerFarmAnimator.PlayerAnimator.GetBool("digBool") == true) { return; }
            if (movementDone) { return; }

            movementDone = true;
            Vector2 dir = playerInput.Player.Move.ReadValue<Vector2>().normalized;

            int x = (int)dir.x;
            int y = (int)dir.y;
            playerInitialPos = gridController.Grid.GetCellCenterWorld(playerPos);
            Vector3Int moveDir = new Vector3Int(x, y, 0) + playerPos;

            int xBoundary = (int)Mathf.Clamp(moveDir.x, gridController.TileMap.cellBounds.min.x, gridController.TileMap.cellBounds.max.x - 1);
            int yBoundary = (int)Mathf.Clamp(moveDir.y, gridController.TileMap.cellBounds.min.y, gridController.TileMap.cellBounds.max.y - 1);

            moveDir = new Vector3Int(xBoundary, yBoundary, 0);

            playerFinalPos = gridController.Grid.GetCellCenterWorld(moveDir);

            playerPos = moveDir;
            OnMovementEvent?.Invoke(dir);
            player.DOMove(playerFinalPos, 0.833f).SetEase(Ease.InOutQuint);
            StartCoroutine(MovementInternalCooldown());
        }
    }

    private IEnumerator MovementInternalCooldown() 
    {
        moveCD = 1.0f;

        yield return new WaitForSeconds(moveCD);

        movementDone = false;
    }

    private void OnFarmPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!FarmController.GetCycleStarted) { return; }
        if (movementDone) { return; }
        if (farmDone) { return; }
        farmDone = true;
        StartCoroutine(FarmInternalCooldown());

        OnFarmInput?.Invoke(true);

    }   

    private void GrabPlants()
    {
        for (int i = plants.Count - 1; i >= 0; --i)
        {
            plants[i].GetComponent<plants>().DestroySelf();
        }
        plants.Clear();
    }

    private IEnumerator FarmInternalCooldown()
    {
        yield return new WaitForSeconds(farmActionCooldown);
        farmDone = false;
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
