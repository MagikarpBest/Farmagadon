using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.GPUSort;
using System.Collections;


public class FarmUIController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] UnityEngine.UI.Slider timeSlider;
    [SerializeField] UnityEngine.UI.Image upcomingEnemy;
    [SerializeField] float maxTime; // seconds
    [SerializeField] GameController gameController;
    [SerializeField] UnityEngine.UI.Image[] bulletPanels;
    
    private float currTime;
    private float gameTime;
    private bool gameStart = false;
    

    private void Awake()
    {
        timeSlider.value = 1;
        currTime = maxTime;
        gameTime = Time.time;
        gameController.gameStart += Game_Start;
        //gameController.nextWave += expandRecommendedList;

    }

    private void Update()
    {
        if (!gameStart) { return; }
        decreaseTime();

    }

    private void Game_Start()
    { 
        gameStart = true;
        setRecommendedList();
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


    private void updateBulletCount()
    {

    }

    private void setRecommendedList()
    {
        UnityEngine.UI.Image[] images = upcomingEnemy.GetComponentsInChildren<UnityEngine.UI.Image>();
        Sprite[] ammoSprites = Resources.LoadAll<Sprite>("ammo");
        Sprite[] enemySprites = Resources.LoadAll<Sprite>("Enemy");
        images[4].sprite = searchSpriteInList("SmallAnt", enemySprites);
        images[7].sprite = searchSpriteInList("Rice", ammoSprites);

    }

    private Sprite searchSpriteInList(string name, Sprite[] spriteList)
    {
        for (int i = 0; i < spriteList.Length; i++)
        {
            print(spriteList[i].name);
            if (spriteList[i].name == name)
            {
                return spriteList[i];
            }
        }
        return spriteList[0];
    }
}
