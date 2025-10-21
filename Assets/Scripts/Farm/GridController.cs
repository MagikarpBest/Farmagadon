using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;

namespace Farm
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] Grid grid;
        [SerializeField] Tilemap tileMap;
        [SerializeField] Vector3Int playerStartPos;
        [SerializeField] CropsData[] cropData;
        [SerializeField] FarmController gameController;
        private List<GameObject> plants = new List<GameObject>();
        private float maxWeight = 100.0f;
        public Grid Grid { get { return grid; } }
        public Tilemap TileMap { get { return tileMap; } }
        public Vector3Int PlayerStartPos { get { return playerStartPos; } }
        
        private void Awake()
        {
            gameController.OnFarmStart += plantCrops;
            gameController.OnFarmEnd += destroyAllPlants; // when timer reaches zero
        }

        private void OnDisable()
        {
            gameController.OnFarmStart -= plantCrops;
            gameController.OnFarmEnd -= destroyAllPlants; 
        }

        private void plantCrops()
        {
            for (int y = tileMap.cellBounds.yMin; y < tileMap.cellBounds.yMax; ++y)
            {
                for (int x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; ++x)
                {
                    CropsData getCrop = pickPlant();
                    GameObject createPlant = Instantiate(getCrop.cropPrefab);
                    plants.Add(createPlant);
                    createPlant.GetComponent<plants>().DropAmount = getCrop.dropAmount;
                    createPlant.GetComponent<plants>().PlantName = getCrop.ammoData;
                    createPlant.GetComponent<plants>().onDestroyed += event_Destroyed;
                    createPlant.GetComponent<plants>().onFarmed += gameController.cropFarmed;
                    createPlant.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(x, y)) + new Vector3(0, createPlant.GetComponent<SpriteRenderer>().size.y / 3, 0);
                    
                }
            }
        }
            
        void event_Destroyed(Vector3 pos)
        {
            if (gameController.StopGame) 
            {
                return; 
            }
            StartCoroutine(createPlantHere(pos));
        }

        IEnumerator createPlantHere(Vector3 pos)
        {
            CropsData chooseCrop = pickPlant();
            yield return new WaitForSeconds(chooseCrop.growRate);
            GameObject createPlant = Instantiate(chooseCrop.cropPrefab);
            plants.Add(createPlant);
            createPlant.GetComponent<plants>().DropAmount = chooseCrop.dropAmount;
            createPlant.GetComponent<plants>().PlantName = chooseCrop.ammoData;
            createPlant.GetComponent<plants>().onDestroyed += event_Destroyed;
            createPlant.GetComponent<plants>().onFarmed += gameController.cropFarmed;
            createPlant.transform.position = pos;
            
        }

        private CropsData pickPlant()
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

        private void destroyAllPlants()
        {
            for (int i = plants.Count-1; i >= 0; i-- )
            {
                Destroy(plants[i]);
            } 
        }
    }
}
