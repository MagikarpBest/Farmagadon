using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float enemySpeed = 1.0f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        rb.linearVelocity = new Vector2(0.0f, -enemySpeed);
    }

    private void Update()
    {
       
    }
}
