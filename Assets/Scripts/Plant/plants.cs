using UnityEngine;
using UnityEngine.Tilemaps;

public class plants : MonoBehaviour
{
    private Vector3Int farmGridLocation;
    private Tilemap TileMap;
    public Vector3Int FarmGridLocation { get { return farmGridLocation; } set { farmGridLocation = value; } }
    public Tilemap tilemap { get { return TileMap; } set { TileMap = value; } }

    public delegate void Destroyed(Vector3 pos);
    public Destroyed onDestroyed;

    void Awake()
    {
    }

    public void destroySelf()
    {
        onDestroyed?.Invoke(gameObject.transform.position);
        Destroy(gameObject);
    }
}
