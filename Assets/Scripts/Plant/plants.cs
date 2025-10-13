using UnityEngine;
using UnityEngine.Tilemaps;

public class plants : MonoBehaviour
{
    private CROP_NAMES plantName;
    private int dropAmount;

    public CROP_NAMES PlantName { set { plantName = value; } }
    public int DropAmount { get { return dropAmount; } set { dropAmount = Mathf.Max(value, 1); } }
    public delegate void Destroyed(Vector3 pos);
    public Destroyed onDestroyed;
    public delegate void Farmed(CROP_NAMES plantName, int dropAmount);
    public Farmed onFarmed;

    void Awake()
    {
    }

    
    public void destroySelf()
    {
        onDestroyed?.Invoke(gameObject.transform.position);
        onFarmed?.Invoke(plantName, dropAmount);
        Destroy(gameObject);
    }
}
