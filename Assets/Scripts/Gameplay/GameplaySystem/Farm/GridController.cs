using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Farm
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private static float maxWeight = 100.0f;
        [SerializeField] private SpriteRenderer flyingCropPrefab;
        [SerializeField] private SpriteRenderer flashTilePrefab;
        private struct PlantData
        {
            public CropsData cropData;
            public int cropDropAmount;
            public float cropGrowChance;
            public int cropGrowRate;
            public PlantData(CropsData data, int dropAmount, float growChance, int growRate)
            {
                cropData = data;
                cropDropAmount = Mathf.Max(dropAmount, 1);
                cropGrowChance = Mathf.Clamp(growChance, 1.0f, maxWeight);
                cropGrowRate = Mathf.Max(growRate, 1);
            }
        };
        [SerializeField] Grid grid;
        [SerializeField] Tilemap tileMap;
        [SerializeField] CropsData[] cropData;
        [SerializeField] FarmController farmController;
        [SerializeField] PlayerFarmInput playerFarmInput;
        [SerializeField] private AudioClip plantGrowAudio;

        private List<GameObject> plants = new List<GameObject>();
        private PlantData[] levelPlantRoster;

        public Grid Grid { get { return grid; } }
        public Tilemap TileMap { get { return tileMap; } }

        private void OnEnable()
        {
            //farmController.StartFarmCycle += PlantCrops;
            farmController.StopFarmCycle += DestroyAllPlants; // when timer reaches zero
            farmController.StartGridPlanting += StartGridController;
        }

        private void OnDisable()
        {
            //farmController.StartFarmCycle -= PlantCrops;
            farmController.StopFarmCycle -= DestroyAllPlants; 
            farmController.StartGridPlanting -= StartGridController;
        }

        private void StartGridController(DayCycleLevelData data)
        {
            if (data == null) return;
            levelPlantRoster = new PlantData[data.cropsEntityList.Length];
            for (int i = 0; i < data.cropsEntityList.Length; ++i) 
            {
                CropEntityData entityData = data.cropsEntityList[i];
                if (entityData == null) return;
                PlantData newPlantData = new(entityData.cropData, entityData.cropDropAmount, entityData.cropGrowChance, entityData.cropGrowRate);
                levelPlantRoster[i] = newPlantData;
            }
            PlantCrops();
        }

        private void PlantCrops()
        {
            for (int y = tileMap.cellBounds.yMin; y < tileMap.cellBounds.yMax; ++y)
            {
                for (int x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; ++x)
                {
                    PlantData getPlant = GetPlantData();
                    InstantiateCrop(getPlant, x, y);
                }
            }
        }

        private void CropDestroyed(int posX, int posY, Vector3 endPos, AmmoData data, BulletPanelUpdater updater, float duration, int dropAmount)
        {
            if (farmController.StopGame)
            {
                return;
            }
            Vector3 startPos = tileMap.GetCellCenterWorld(new Vector3Int(posX, posY));
            StartCoroutine(PlayFlyingCrop(startPos, endPos, data, duration, dropAmount, updater));
            StartCoroutine(CreatePlantHere(posX, posY));
            StartCoroutine(CreateFlashTile(posX, posY));
            
        }

        private IEnumerator CreateFlashTile(int posX, int posY)
        {
            Vector3 startPos = tileMap.GetCellCenterWorld(new Vector3Int(posX, posY));
            SpriteRenderer flashTile = Instantiate(flashTilePrefab, startPos, Quaternion.identity);
            flashTile.GetComponent<FlashTile>().DoFlash();
            yield return null; 
        }

        private IEnumerator CreatePlantHere(int posX, int posY)
        {
            
            PlantData plantData = GetPlantData();
            
            yield return new WaitForSeconds(plantData.cropGrowRate + Random.Range(Mathf.Max(1, plantData.cropGrowRate-3),3));
            InstantiateCrop(plantData, posX, posY);

        }

        private IEnumerator PlayFlyingCrop(Vector3 startPos, Vector3 endPos, AmmoData data, float duration, int dropAmount, BulletPanelUpdater updater)
        {
            SpriteRenderer flyingCropSprite = Instantiate(flyingCropPrefab);
            flyingCropSprite.sprite = data.cropIcon;
            flyingCropSprite.transform.position = startPos;
            float offset = 4.0f;
            float midpoint = ((endPos - startPos) / 2.0f).x;
            int posY = playerFarmInput.PlayerPos.y;

            offset = Mathf.Abs(posY * 0.5f) + 0.5f;
            
            Vector3[] curvePath = new[] { startPos, startPos + new Vector3(midpoint/3, offset), startPos + new Vector3(midpoint/2, offset), startPos + new Vector3(midpoint, offset), endPos };
            Sequence flyingSequence = DOTween.Sequence();
            flyingSequence.Append(flyingCropSprite.transform.DOMove(endPos, 1.0f));
            //flyingSequence.Append(flyingCropSprite.transform.DOPath(curvePath, duration, PathType.Linear));
            flyingSequence.Append(flyingCropSprite.transform.DOScale(0.5f, 0.2f));
            
            yield return new WaitForSeconds(flyingSequence.Duration()+0.2f);
            farmController.CropFarmed(data, dropAmount);
            if (updater != null)
            {
                updater.UpdateSelf();
            }
            Destroy(flyingCropSprite.gameObject);
        }


        private PlantData GetPlantData()
        {
            foreach (PlantData plant in levelPlantRoster)
            {
                float randomNum = Random.Range(0.0f, maxWeight);
                if (plant.cropGrowChance >= randomNum)
                {
                    return plant;
                }
            }
            return levelPlantRoster[Random.Range(0, levelPlantRoster.Length-1)];
        } 

        private void InstantiateCrop(PlantData data, int posX, int posY)
        {
            GameObject createPlant = Instantiate(data.cropData.cropPrefab);
            plants.Add(createPlant);
            createPlant.GetComponent<plants>().DropAmount = data.cropDropAmount;
            createPlant.GetComponent<plants>().PlantAmmoData = data.cropData.ammoData;
            createPlant.GetComponent<plants>().OnDestroyed += CropDestroyed;
            //createPlant.GetComponent<plants>().OnFarmed += farmController.CropFarmed;
            createPlant.GetComponent<plants>().PosX = posX;
            createPlant.GetComponent<plants>().PosY=  posY;
            createPlant.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(posX, posY)) + new Vector3(0, createPlant.GetComponentInChildren<SpriteRenderer>().size.y / 3, 0);
            AudioService.AudioManager.BufferPlayOneShot(plantGrowAudio);
        }

        private void DestroyAllPlants()
        {
            for (int i = plants.Count - 1; i >= 0; i--)
            {
                Destroy(plants[i]);
            }
            
        }
    }
}