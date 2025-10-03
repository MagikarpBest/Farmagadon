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
        [SerializeField] GameObject plantPrefab;
        private float growCooldown;
        public Grid Grid { get { return grid; } }
        public Tilemap TileMap { get { return tileMap; } }
        public Vector3Int PlayerStartPos { get { return playerStartPos; } }

        void Start()
        {
            for (int y = tileMap.cellBounds.yMin; y<tileMap.cellBounds.yMax; ++y)
            {
                for (int x = tileMap.cellBounds.xMin; x<tileMap.cellBounds.xMax; ++x)
                {
                    GameObject createPlant = Instantiate(plantPrefab);
                    createPlant.GetComponent<plants>().FarmGridLocation = new Vector3Int(x, y);
                    createPlant.GetComponent<plants>().tilemap = tileMap;
                    createPlant.GetComponent<plants>().onDestroyed += event_Destroyed;
                    createPlant.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(x, y));
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
            GameObject createPlant = Instantiate(plantPrefab);
            createPlant.GetComponent<plants>().onDestroyed += event_Destroyed;
            createPlant.transform.position = pos;
        }
    }
}
