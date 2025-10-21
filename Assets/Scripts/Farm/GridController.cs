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
        [SerializeField] GameController gameController;
        private float maxWeight = 100.0f;
        private float growCooldown;
        public Grid Grid { get { return grid; } }
        public Tilemap TileMap { get { return tileMap; } }
        public Vector3Int PlayerStartPos { get { return playerStartPos; } }
        
        private void Awake()
        {
           
        }
        void Start()
        {
            for (int y = tileMap.cellBounds.yMin; y<tileMap.cellBounds.yMax; ++y)
            {
                for (int x = tileMap.cellBounds.xMin; x<tileMap.cellBounds.xMax; ++x)
                {
                    CropsData getCrop = pickPlant();
                    GameObject createPlant = Instantiate(getCrop.cropPrefab);
                    createPlant.GetComponent<plants>().DropAmount = getCrop.dropAmount;
                    createPlant.GetComponent<plants>().PlantName = getCrop.ammoData;
                    createPlant.GetComponent<plants>().onDestroyed += event_Destroyed;
                    createPlant.GetComponent<plants>().onFarmed += gameController.cropFarmed;
                    createPlant.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(x, y)) + new Vector3(0, createPlant.GetComponent<SpriteRenderer>().size.y/3, 0);
                }
            }
        }
            
        void event_Destroyed(Vector3 pos)
        {

            StartCoroutine(createPlantHere(pos));
        }

        IEnumerator createPlantHere(Vector3 pos)
        {
            CropsData chooseCrop = pickPlant();
            yield return new WaitForSeconds(chooseCrop.growRate);
            GameObject createPlant = Instantiate(chooseCrop.cropPrefab);
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
    }
}
