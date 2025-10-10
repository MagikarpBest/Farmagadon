using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Collections.Generic;

public class FarmUIController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] UnityEngine.UI.Slider timeSlider;
    [SerializeField] RectTransform upcomingEnemy;
    [SerializeField] UnityEngine.UI.Image enemyIconPrefab;
    [SerializeField] UnityEngine.UI.Image bulletIconPrefab;
    [SerializeField] float maxTime; // seconds
    [SerializeField] GameController gameController;
    private float currTime;
    private float gameTime;
    private bool gameStart = false;
    private List<List<string>> upcomingEnemyList = new List<List<string>>(); 

    private void Awake()
    {
        timeSlider.value = 1;
        currTime = maxTime;
        gameTime = Time.time;
        gameController.gameStart += Game_Start;
        gameController.nextWave += expandRecommendedList;

    }

    private void Update()
    {
        if (!gameStart) { return; }
        decreaseTime();

    }

    private void Game_Start()
    { 
        gameStart = true;
    }
    private void decreaseTime()
    {
        
        if (currTime <= 0.0f)
        {
            gameStart = false;
            return;
        }

        if (Time.time - gameTime >= 1.0f)
        {
            gameTime = Time.time;
            float timeNormalized = currTime / maxTime;
            timeSlider.value = timeNormalized;
            currTime -= 1.0f;
        }
    }

    private void setRecommendedList()
    {

    }

    private void expandRecommendedList(List<string> enemies)
    {

        RectTransform bulletRect = bulletIconPrefab.GetComponent<RectTransform>();
        float bulletHeight = bulletRect.rect.height;
        float rowSpacing = 110f; 
        float verticalIconOffset = 50f; 
        float horizontalOffset = 870f; 

        
        float yOffset = 0f;

        
        foreach (string enemy in enemies)
        {
            
            Vector2 panelSizeIncrease = new Vector2(0, bulletHeight);
            upcomingEnemy.sizeDelta += panelSizeIncrease;

            
            upcomingEnemy.anchoredPosition -= new Vector2(0, bulletHeight - verticalIconOffset);

            
            float iconY = verticalIconOffset + yOffset;

            
            UnityEngine.UI.Image enemyIcon = Instantiate(enemyIconPrefab, canvas.transform);
            RectTransform enemyRT = enemyIcon.GetComponent<RectTransform>();
            enemyRT.anchoredPosition = new Vector2(-horizontalOffset, iconY+100);

            
            UnityEngine.UI.Image bulletIcon = Instantiate(bulletIconPrefab, canvas.transform);
            RectTransform bulletRT = bulletIcon.GetComponent<RectTransform>();
            bulletRT.anchoredPosition = new Vector2(-horizontalOffset+230, iconY+100);

            
            yOffset -= rowSpacing;
        }
    }
}
