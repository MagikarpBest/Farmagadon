using UnityEngine;
using UnityEngine.UI;

public class RecommendedPanelHandler : MonoBehaviour
{
    [SerializeField] FarmController gameController;
    [SerializeField] Image enemyImage1;
    [SerializeField] Image enemyImage2;
    [SerializeField] Image enemyImage3;
    [SerializeField] Image bulletImage1;
    [SerializeField] Image bulletImage2;
    [SerializeField] Image bulletImage3;

    //private EnemyMatchupData data = new();

    private void Awake()
    {
        
        gameController.OnGetRecommended += placeSprites;
        
        
    }

    private void OnDisable()
    {
        if (gameController.OnFarmStart != null)
        {
            gameController.OnGetRecommended -= placeSprites;
        }
    }

    private void placeSprites(ENEMY_WEAKNESS[] placeholderParam)
    {
        
    }
}
