using UnityEngine;
using UnityEngine.Tilemaps;

public class plants : MonoBehaviour
{
    private Vector3Int farmGridLocation;
    private Tilemap TileMap;
    public int growRate;
    public Vector3Int FarmGridLocation { get { return farmGridLocation; } set { farmGridLocation = value; } }
    public Tilemap tilemap { get { return TileMap; } set { TileMap = value; } }

    public delegate void Destroyed(Vector3 pos, int growRate);
    public Destroyed onDestroyed;

    void Awake()
    {
        growRate = (int)Random.Range(1.0f, 5.0f);
    }

    public void destroySelf()
    {
        onDestroyed?.Invoke(gameObject.transform.position, growRate);
        Destroy(gameObject);
    }
}
