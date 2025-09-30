using NUnit.Framework.Internal;
using System.ComponentModel.Design.Serialization;
using System.Net.NetworkInformation;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Farm
{


    public class GridController : MonoBehaviour
    {
        [SerializeField] Grid grid;
        [SerializeField] GameObject tilePrefab;

        [SerializeField] int width;
        [SerializeField] int height;

        [SerializeField] Vector3Int playerStartPos;

        public Grid Grid { get { return grid; } }
        public int Width { get { return width; } set { width = (int)Mathf.Clamp(value, -3, 4); } }
        public int Height { get { return height; } set { height = (int)Mathf.Clamp(value, -3, 4); } }
        public Vector3Int PlayerStartPos { get { return playerStartPos; } set { playerStartPos = value; } }

        private int origin = -4;
        public int Origin { get { return origin; } }
        void Start()
        {
            for (int y = origin; y < origin + height; ++y)
            {
                for (int x = origin; x < origin + width; ++x)
                {
                    Vector3 tilePos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    Instantiate(tilePrefab, tilePos, Quaternion.identity);
                }
            }
        }

        private void OnValidate()
        {
            width = (int)Mathf.Clamp(width, -3, 4);
            height = (int)Mathf.Clamp(height, -3, 4);
        }
    }
}
