using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Farm
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private static float maxWeight = 100.0f;
        public struct plantData
        {
            private CropsData cropData;
            private int cropDropAmount;
            private float cropGrowChance;
            private int cropGrowRate;
            public plantData(CropsData data, int dropAmount, float growChance, int growRate)
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
        private plantData[] levelPlantData;

        public Grid Grid { get { return grid; } }
        public Tilemap TileMap { get { return tileMap; } }
        public Vector3Int PlayerStartPos { get { return playerStartPos; } }

        private void OnEnable()
        {
            farmController.StartFarmCycle += PlantCrops;
            farmController.StopFarmCycle += DestroyAllPlants; // when timer reaches zero
        }

        private void OnDisable()
        {
            farmController.StartFarmCycle -= PlantCrops;
            farmController.StopFarmCycle -= DestroyAllPlants;
        }

        private void StartGridController()
        {
         
        }

        private void PlantCrops()
        {
            for (int y = tileMap.cellBounds.yMin; y < tileMap.cellBounds.yMax; ++y)
            {
                for (int x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; ++x)
                {
                    CropsData getCrop = PickPlant();
                    GameObject createPlant = Instantiate(getCrop.cropPrefab);
                    plants.Add(createPlant);
                    createPlant.GetComponent<plants>().DropAmount = getCrop.dropAmount;
                    createPlant.GetComponent<plants>().PlantName = getCrop.ammoData;
                    createPlant.GetComponent<plants>().OnDestroyed += CropDestroyed;
                    createPlant.GetComponent<plants>().OnFarmed += farmController.cropFarmed;
                    createPlant.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(x, y)) + new Vector3(0, createPlant.GetComponent<SpriteRenderer>().size.y / 3, 0);
                }
            }
        }

        private void CropDestroyed(Vector3 pos)
        {
            if (farmController.StopGame)
            {
                return;
            }
            StartCoroutine(CreatePlantHere(pos));
        }

        private IEnumerator CreatePlantHere(Vector3 pos)
        {
            CropsData chooseCrop = PickPlant();
            yield return new WaitForSeconds(chooseCrop.growRate);
            GameObject createPlant = Instantiate(chooseCrop.cropPrefab);
            plants.Add(createPlant);
            createPlant.GetComponent<plants>().DropAmount = chooseCrop.dropAmount;
            createPlant.GetComponent<plants>().PlantName = chooseCrop.ammoData;
            createPlant.GetComponent<plants>().OnDestroyed += CropDestroyed;
            createPlant.GetComponent<plants>().OnFarmed += farmController.cropFarmed;
            createPlant.transform.position = pos;

        }

        /*
        private plantData[] GetPlantData(DayCycleLevelData data)
        {
            plantData[] plantDataList = new plantData[data.cropsEntityList.Length];
            for (int i = 0; i < data.cropsEntityList.Length; ++i)
            {
                CropsData cropData = data.cropsEntityList[i].cropData;
                int cropDropAmount = data.cropsEntityList[i].cropDropAmount;
                float cropGrowChance = data.cropsEntityList[i].cropGrowChance;
                int cropGrowRate = data.cropsEntityList[i].cropGrowRate;
                plantData newPlantData = new plantData(cropData, cropDropAmount, cropGrowChance, cropGrowRate);
                plantDataList[i] = newPlantData;
            }
            return plantDataList;
        } */

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

        private void DestroyAllPlants()
        {
            for (int i = plants.Count - 1; i >= 0; i--)
            {
                Destroy(plants[i]);
            }
        }
    }
}