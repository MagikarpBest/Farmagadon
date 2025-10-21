using UnityEngine;
using UnityEngine.Tilemaps;

public class plants : MonoBehaviour
{
    private AmmoData plantName;
    private int dropAmount;

    public AmmoData PlantName { set { plantName = value; } }
    public int DropAmount { get { return dropAmount; } set { dropAmount = Mathf.Max(value, 1); } }
    public delegate void Destroyed(Vector3 pos);
    public Destroyed onDestroyed;
    public delegate void Farmed(AmmoData plantName, int dropAmount);
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
