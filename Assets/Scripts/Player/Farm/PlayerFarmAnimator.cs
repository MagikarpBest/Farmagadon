using Unity.VisualScripting;
using UnityEngine;

public class PlayerFarmAnimator : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] PlayerFarmInput farmInput;

    private void Awake()
    {
        farmInput.OnFarmInput += setFarmBool;
    }

    private void setFarmBool(bool farming)
    {
        playerAnimator.SetBool("farming", farming);
        
    }

    public void farmBool()
    {
        playerAnimator.SetBool("farming", false);
    }
}
