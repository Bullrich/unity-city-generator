using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By @Bullrich
namespace CityGenerator
{
    public class BuildCity : MonoBehaviour
    {
        public GameObject[] buildings;

        // https://youtu.be/sLtelfckEjc?t=13m6s
        public GameObject
            xStreets,
            zStreets,
            crossRoad;

        public int
            mapWidth = 20,
            mapHeight = 20;

        private int buildingFootprint = 3;
        private int[,] mapGrid;

        void Start()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            mapGrid = new int[mapWidth, mapHeight];

            float seed = Random.Range(0, 100);

            // generate map data
            for (int h = 0; h < mapHeight; h++)
            {
                for (int w = 0; w < mapWidth; w++)
                {
                    mapGrid[w, h] = (int) (Mathf.PerlinNoise(w / 10f + seed, h / 10f + seed) * 10);
                }
            }

            // build streets
            int x = 0;
            for (int n = 0; n < 50; n++)
            {
                for (int h = 0; h < mapHeight; h++)
                {
                    mapGrid[x, h] = -1;
                }
                x += Random.Range(2, 15);
                if (x >= mapWidth) break;
            }

            int z = 0;
            for (int n = 0; n < 10; n++)
            {
                for (int w = 0; w < mapWidth; w++)
                {
                    if (mapGrid[w, z] == -1)
                        mapGrid[w, z] = -3;
                    else
                        mapGrid[w, z] = -2;
                }
                z += Random.Range(5, 20);
                if (z >= mapHeight) break;
            }

            // build city
            for (int h = 0; h < mapHeight; h++)
            {
                for (int w = 0; w < mapWidth; w++)
                {
                    int result = mapGrid[w, h];
                    Vector3 pos = new Vector3(w * buildingFootprint, 0, h * buildingFootprint);
                    if (result < -2)
                        Instantiate(crossRoad, pos, crossRoad.transform.rotation);
                    else if (result < -1)
                        Instantiate(xStreets, pos, xStreets.transform.rotation);
                    else if (result < 0)
                        Instantiate(zStreets, pos, zStreets.transform.rotation);
                    else
                        Instantiate(GetNoiseGO(result), pos, Quaternion.identity);
                }
            }
        }

        private GameObject GetNoiseGO(float result)
        {
            if (result < 2)
                return buildings[0];
            else if (result < 4)
                return buildings[1];
            else if (result < 5)
                return buildings[2];
            else if (result < 8)
                return buildings[3];
            else if (result < 7)
                return buildings[4];
            else
                return buildings[5];

            // https://youtu.be/xkuniXI3SEE?t=14m2s
        }
    }
}