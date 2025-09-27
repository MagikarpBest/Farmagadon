using UnityEngine;

public class Bullets : MonoBehaviour
{

    [SerializeField] float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If collided with stuff check isit enemy, if yes then deal damage.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
