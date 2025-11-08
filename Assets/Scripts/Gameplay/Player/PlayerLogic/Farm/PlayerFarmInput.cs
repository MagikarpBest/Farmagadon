using Farm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerFarmInput : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private BoxCollider2D cropRadius;
    [SerializeField] private GridController gridController;
    [SerializeField] private float farmActionCooldown = 1.0f;

    
    private List<GameObject> plants = new List<GameObject>();

    private bool movementDone = false;
    private bool farmDone = false;
    
    private float startTime = 0;
    private float t = 0;
    private float faCD = 0.0f;
    private float moveCD = 0.0f;

    private PlayerInput playerInput;
    Vector3Int playerPos = new();
    private Vector3 playerInitialPos = Vector3.zero;
    private Vector3 playerFinalPos = Vector3.zero;

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
        playerInput.Player.Move.performed += OnFarmMovement;
    }

    private void OnDestroy()
    {
        playerInput.Player.Shoot.performed -= OnFarmPerformed;
        playerInput.Player.Move.performed -= OnFarmMovement;

    }
    private void OnFarmMovement(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (movementDone) { return; }
        Vector2 dir = playerInput.Player.Move.ReadValue<Vector2>().normalized;
        OnMovementEvent?.Invoke(dir);
        int x = (int)dir.x;
        int y = (int)dir.y;
        playerInitialPos = gridController.Grid.GetCellCenterWorld(playerPos);
        Vector3Int moveDir = new Vector3Int(x, y, 0) + playerPos;

        int xBoundary = (int)Mathf.Clamp(moveDir.x, gridController.TileMap.cellBounds.min.x, gridController.TileMap.cellBounds.max.x - 1);
        int yBoundary = (int)Mathf.Clamp(moveDir.y, gridController.TileMap.cellBounds.min.y, gridController.TileMap.cellBounds.max.y - 1);

        moveDir = new Vector3Int(xBoundary, yBoundary, 0);

        playerFinalPos = gridController.Grid.GetCellCenterWorld(moveDir);

        playerPos = moveDir;
        movementDone = true;
        player.DOMove(playerFinalPos, 0.5f).SetEase(Ease.InOutQuint); 
        StartCoroutine(MovementInternalCooldown());
    }

    private IEnumerator MovementInternalCooldown() 
    {
        moveCD = farmActionCooldown;

        while (moveCD >= 0)
        {
            moveCD -= Time.deltaTime;
            yield return null;
        }

        movementDone = false;
    }


    private void Update()
    {
        /*
        if (movementDone)
        {
            if (Time.time - startTime >= 0.05f)
            {
                t += 0.2f;
                startTime = Time.time;
                
                player.position = Vector3.Lerp(playerInitialPos, playerFinalPos, t);
            }
            if (t >= 1.0f)
            {
                t -= 1.0f;
                movementDone = false;
            }
        } */
    }
    private void OnFarmPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (movementDone) { return; }
        if (farmDone) { return; }
        farmDone = true;
        StartCoroutine(FarmInternalCooldown());
        OnFarmInput?.Invoke(true);
        for (int i = plants.Count - 1; i >= 0; --i)
        {
            plants[i].GetComponent<plants>().DestroySelf();
        }
        plants.Clear();
        
    }   

    private IEnumerator FarmInternalCooldown()
    {
        faCD = farmActionCooldown;
        
        while (faCD >= 0.0f)
        {
            faCD -= Time.deltaTime;
            yield return null;
        }
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
