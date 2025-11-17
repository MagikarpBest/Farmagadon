using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class plants : MonoBehaviour
{
    [SerializeField] FlashEffect flashEffect;
    [SerializeField] AudioClip farmedAudio;


    private AmmoData plantAmmoData;
    private int dropAmount;
    private int posX;
    private int posY;

    public AmmoData PlantAmmoData { set { plantAmmoData = value; } }
    public int DropAmount { get { return dropAmount; } set { dropAmount = Mathf.Max(value, 1); } }
    public int PosX { set { posX = value; } }
    public int PosY { set { posY = value; } }


    public delegate void Destroyed(int posX, int posY, Vector3 endPos, AmmoData data, BulletPanelUpdater updater, float duration=1.0f, int dropAmount = 0);
    public Destroyed OnDestroyed;
    public delegate void Farmed(AmmoData plantName, int dropAmount);
    public Farmed OnFarmed;
    
    public void DestroySelf()
    {
        
        //OnFarmed?.Invoke(plantAmmoData, dropAmount);
        flashEffect.CallDamageFlash();
        Sequence plantShrink = DOTween.Sequence();
        plantShrink.Prepend(transform.DOPunchPosition(new Vector3(0.1f, 0.0f, 0.0f), 0.5f));
        plantShrink.Prepend(GetComponentInChildren<SpriteRenderer>().DOFade(0, 0.5f));
        plantShrink.Prepend(transform.DOScale(0, 0.5f));
        AudioService.AudioManager.BufferPlayOneShot(farmedAudio, 0.5f);
        Vector3 panelPos = Vector3.zero;
        Image panel = BulletPanelHandler.Instance.GetBulletPanel(plantAmmoData);
        BulletPanelUpdater updater = null; // Create a variable for the updater

        // --- Start Safety Checks ---
        if (panel == null)
        {
            Debug.LogWarning($"[plants] GetBulletPanel returned null for {plantAmmoData.name}. UI animation skipped.");
        }
        else
        {
            // Panel exists, now try to get the updater
            updater = panel.GetComponent<BulletPanelUpdater>();
            if (updater == null)
            {
                Debug.LogWarning($"[plants] Panel '{panel.name}' is missing the BulletPanelUpdater component.");
            }
            else if (updater.AmmoImage == null) // Also check the image inside the updater!
            {
                Debug.LogWarning($"[plants] BulletPanelUpdater's 'AmmoImage' is not assigned (null).");
                updater = null; // Set updater to null so we don't try to use it
            }
            else
            {
                // SUCCESS! All components are valid.
                panelPos = updater.AmmoImage.position;
            }
        }
        // --- End Safety Checks ---

        // Now, the Invoke is safe.
        // 'updater' will either be the valid component or null.
        // 'panelPos' will either be the correct position or Vector3.zero.
        OnDestroyed?.Invoke(posX, posY, panelPos, plantAmmoData, updater, 1.0f, dropAmount);

        StartCoroutine(destroyAfter(plantShrink.Duration()));
    }

    private IEnumerator destroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        Destroy(gameObject);
    }
}
