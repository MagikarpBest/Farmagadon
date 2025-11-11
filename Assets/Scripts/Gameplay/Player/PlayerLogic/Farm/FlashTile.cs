using UnityEngine;
using System.Collections;
public class FlashTile : MonoBehaviour
{
    [SerializeField] private FlashEffect flashEffect;

    public void DoFlash()
    {
        flashEffect.CallDamageFlash();
        StartCoroutine(DeleteSelf());
    }

    private IEnumerator DeleteSelf()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
