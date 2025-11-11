using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class RecomPanelHandler : MonoBehaviour
{
    [SerializeField] private FarmController farmController;
    [SerializeField] private Image enemyIconPrefab;
    [SerializeField] private Image[] bulletImages;
    private UpcomingEnemyData upcomingEnemyData;

    private void OnEnable()
    {
        farmController.SetUpcomingEnemies += UpdateRecomPanel;
    }

    private void UpdateRecomPanel(DayCycleLevelData data)
    {
        upcomingEnemyData = data.upcomingEnemyDatas;
        for (int i = 0; i < bulletImages.Length; ++i)
        {
            //HandleEnemyIcons(i);
            bulletImages[i].sprite = upcomingEnemyData.upcomingEnemyDatas[i].enemyWeakness;
        }
    }

    /*
    private void HandleEnemyIcons(int index)
    {
        UpcomersData[] data = upcomingEnemyData.upcomingEnemyDatas;
        for (int i  = 0; i < (int)data[index].density; ++i)
        {
            Image dupedImagePrefab = Instantiate(enemyIconPrefab, enemyImages[index].transform);
            dupedImagePrefab.rectTransform.anchoredPosition = new Vector2(Mathf.Max(10*i,10), Mathf.Min(-10*i, -10));
            dupedImagePrefab.sprite = data[index].enemySprite;
            
        }
        enemyImages[index].sprite = data[index].enemySprite;
    }*/
}
