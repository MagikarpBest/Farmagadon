using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Net.NetworkInformation;
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
        [SerializeField] GameObject potatoPrefab;
        [SerializeField] GameObject carrotPrefab;
        [SerializeField] GameObject cornPrefab;
        private float growCooldown;
        public Grid Grid { get { return grid; } }
        public Tilemap TileMap { get { return tileMap; } }
        public Vector3Int PlayerStartPos { get { return playerStartPos; } }
        float corn;
        float potato;
        float carrot;
        private void Awake()
        {
            float maxWeight = 100;
            float rand = Random.Range(1, maxWeight);
            maxWeight -= rand;
            corn = rand;
            rand = Random.Range(1, maxWeight);
            maxWeight -= rand;
            potato = rand;
            carrot = maxWeight;


        }
        void Start()
        {
            

            for (int y = tileMap.cellBounds.yMin; y<tileMap.cellBounds.yMax; ++y)
            {
                for (int x = tileMap.cellBounds.xMin; x<tileMap.cellBounds.xMax; ++x)
                {
                    GameObject createPlant = Instantiate(pickPlant());
                    createPlant.GetComponent<plants>().FarmGridLocation = new Vector3Int(x, y);
                    createPlant.GetComponent<plants>().tilemap = tileMap;
                    createPlant.GetComponent<plants>().onDestroyed += event_Destroyed;
                    
                    createPlant.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(x, y)) + new Vector3(0, createPlant.GetComponent<SpriteRenderer>().size.y/3, 0);
                }
            }
        }

        void event_Destroyed(Vector3 pos,int growRate)
        {
            StartCoroutine(createPlantHere(pos, growRate));
            
        }

        IEnumerator createPlantHere(Vector3 pos, int growRate)
        {
            yield return new WaitForSeconds(growRate);
            GameObject createPlant = Instantiate(pickPlant());
            createPlant.GetComponent<plants>().onDestroyed += event_Destroyed;
            createPlant.transform.position = pos;
        }

        private GameObject pickPlant()
        {
            float weight = Random.Range(1, 100);
            if (weight > corn)
            {
                return cornPrefab;
            }
            else if (weight > carrot)
            {
                return carrotPrefab;
            }
            else if (weight > potato)
            {
                return potatoPrefab;
            }
            else
            {
                return potatoPrefab;
            }
        }
    }
}
