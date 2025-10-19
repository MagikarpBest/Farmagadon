using UnityEngine;
using UnityEngine.UI;

public class RecommendedPanelHandler : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] Image enemyImage1;
    [SerializeField] Image enemyImage2;
    [SerializeField] Image enemyImage3;
    [SerializeField] Image bulletImage1;
    [SerializeField] Image bulletImage2;
    [SerializeField] Image bulletImage3;

    private EnemyMatchupData data = new();

    private void Awake()
    {
        
        gameController.OnGetRecommended += placeSprites;
        
        
    }

    private void OnDisable()
    {
        if (gameController.gameStart != null)
        {
            gameController.OnGetRecommended -= placeSprites;
        }
    }

    private void placeSprites(ENEMY_WEAKNESS[] placeholderParam)
    {
        enemyImage1.sprite = Resources.Load<Sprite>(data.enemyMatchupDict[placeholderParam[0]]);
        enemyImage2.sprite = Resources.Load<Sprite>(data.enemyMatchupDict[placeholderParam[1]]);
        enemyImage3.sprite = Resources.Load<Sprite>(data.enemyMatchupDict[placeholderParam[2]]);    
        bulletImage1.sprite = Resources.Load<Sprite>(data.bulletPathDict[placeholderParam[0]]);
        bulletImage2.sprite = Resources.Load<Sprite>(data.bulletPathDict[placeholderParam[1]]);
        bulletImage3.sprite = Resources.Load<Sprite>(data.bulletPathDict[placeholderParam[2]]);
    }
}
