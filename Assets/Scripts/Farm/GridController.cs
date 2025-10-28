using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Farm
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private static float maxWeight = 100.0f;
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
        [SerializeField] Vector3Int playerStartPos;
        [SerializeField] CropsData[] cropData;
        [SerializeField] FarmController farmController;
        private List<GameObject> plants = new List<GameObject>();
        private PlantData[] levelPlantRoster;

        public Grid Grid { get { return grid; } }
        public Tilemap TileMap { get { return tileMap; } }
        public Vector3Int PlayerStartPos { get { return playerStartPos; } }

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

        private void CropDestroyed(int posX, int posY)
        {
            if (farmController.StopGame)
            {
                return;
            }
            StartCoroutine(CreatePlantHere(posX, posY));
        }

        private IEnumerator CreatePlantHere(int posX, int posY)
        {
            
            PlantData plantData = GetPlantData();
            yield return new WaitForSeconds(plantData.cropGrowRate);
            InstantiateCrop(plantData, posX, posY);

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

        private CropsData PickPlant()
        {
            foreach (CropsData crop in cropData)
            {
                float randomNum = Random.Range(1, maxWeight);
                if (crop.cropWeightage >= randomNum)
                {
                    return crop;
                }
            }
            return cropData[Random.Range(1, cropData.Length - 1)];

        }

        private void InstantiateCrop(PlantData data, int posX, int posY)
        {
            GameObject createPlant = Instantiate(data.cropData.cropPrefab);
            plants.Add(createPlant);
            createPlant.GetComponent<plants>().DropAmount = data.cropDropAmount;
            createPlant.GetComponent<plants>().PlantAmmoData = data.cropData.ammoData;
            createPlant.GetComponent<plants>().OnDestroyed += CropDestroyed;
            createPlant.GetComponent<plants>().OnFarmed += farmController.cropFarmed;
            createPlant.GetComponent<plants>().PosX = posX;
            createPlant.GetComponent<plants>().PosY=  posY;
            createPlant.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(posX, posY)) + new Vector3(0, createPlant.GetComponent<SpriteRenderer>().size.y / 3, 0);
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