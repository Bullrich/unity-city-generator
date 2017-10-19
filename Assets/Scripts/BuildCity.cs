using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

// By @Bullrich
namespace CityGenerator
{
    public class BuildCity : MonoBehaviour
    {
        public GameObject[] buildings;

        public GameObject
            xStreets,
            zStreets,
            crossRoad;

        public int
            mapWidth = 20,
            mapHeight = 20;

        private int buildingFootprint = 3;

        private GameObject container;

        private Lot[,] map;

        private void Start()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            map = GenerateGrid(Random.Range(0, 50));
            InstantiateGridElements(map);
            CalculateCityLots(map);
        }

        private Lot[,] GenerateGrid(float seed)
        {
            Lot[,] grid = new Lot[mapWidth, mapHeight];

            // build streets
            int x = 0;
            for (int n = 0; n < 50; n++)
            {
                for (int h = 0; h < mapHeight; h++)
                {
                    grid[x, h] = new Lot(true, new Vector2(x, h), -1);
                }
                x += Random.Range(2, 15);
                if (x >= mapWidth) break;
            }

            int z = 0;
            for (int n = 0; n < 10; n++)
            {
                for (int w = 0; w < mapWidth; w++)
                {
                    if (grid[w, z] != null && grid[w, z].street)
                        grid[w, z].lotType = -3;
                    else
                        grid[w, z] = new Lot(true, new Vector2(w, z), -2);
                }
                z += Random.Range(2, 20);
                if (z >= mapHeight) break;
            }

            // add buildings
            for (int h = 0; h < mapHeight; h++)
            {
                for (int w = 0; w < mapWidth; w++)
                {
                    if (grid[w, h] == null || !grid[w, h].street)
                    {
                        float perlin = Mathf.PerlinNoise(w / 10f + seed, h / 10f + seed) * 10;
                        grid[w, h] = new Lot(false, new Vector2(w, h),
                            perlin);
                    }
                    grid[w,h].worldPos = new Vector3(w * buildingFootprint, 0, h * buildingFootprint);
                }
            }

            return grid;
        }

        private void InstantiateGridElements(Lot[,] lots)
        {
            container = new GameObject("CityContainer");
            container.transform.position = Vector3.zero;
            for (int x = 0; x < lots.GetLength(0); x++)
            {
                for (int z = 0; z < lots.GetLength(1); z++)
                {
                    Lot lot = lots[x, z];
                    float result = lot.lotType;
                    GameObject lotGO;
                    if (result < -2)
                        lotGO = Instantiate(crossRoad, lot.worldPos, crossRoad.transform.rotation);
                    else if (result < -1)
                        lotGO = Instantiate(xStreets, lot.worldPos, xStreets.transform.rotation);
                    else if (result < 0)
                        lotGO = Instantiate(zStreets, lot.worldPos, zStreets.transform.rotation);
                    else
                        lotGO = Instantiate(GetElementFromNoise(result), lot.worldPos, Quaternion.identity);
                    lotGO.transform.SetParent(container.transform);
                }
            }
        }

        private void CalculateCityLots(Lot[,] lots)
        {
            for (int x = 0; x < lots.GetLength(0); x++)
            {
                for (int z = 0; z < lots.GetLength(1); z++)
                {
                    Lot lot = lots[x, z];
                    if (!lot.street && lot.neighrboor == null)
                        GenerateApple(lots, x, z);
                }
            }
        }

        private List<Lot> GetApple(Lot lot, List<Lot> apple)
        {
            if (lot == null || lot.street) return apple;
            apple.Add(lot);
            GetApple(lot.neighrboor, apple);
            return apple;
        }

        private void GenerateApple(Lot[,] lots, int x, int z)
        {
            Lot thisLot = lots[x, z];
            if (x + 1 < lots.GetLength(0))
            {
                Lot xNeighboor = lots[x + 1, z];
                if (!xNeighboor.street && xNeighboor.neighrboor == null)
                {
                    xNeighboor.neighrboor = thisLot;
                    GenerateApple(lots, x + 1, z);
                }
            }
            if (z + 1 < lots.GetLength(1))
            {
                Lot zNeighboor = lots[x, z + 1];
                if (!zNeighboor.street && zNeighboor.neighrboor == null)
                {
                    zNeighboor.neighrboor = thisLot;
                    GenerateApple(lots, x, z + 1);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Destroy(container);
                container = null;
                System.GC.Collect();
                GenerateMap();
            }
            else if (Input.GetKey(KeyCode.M))
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int z = 0; z < map.GetLength(1); z++)
                    {
                        Lot lot = map[x, z];
                        if (lot.neighrboor != null)
                        {
                            Vector3 thisPos = new Vector3(lot.gridPos.x * buildingFootprint, 0,
                                lot.gridPos.y * buildingFootprint);
                            Vector3 neighboorPos = new Vector3(lot.neighrboor.gridPos.x * buildingFootprint, 0,
                                lot.neighrboor.gridPos.y * buildingFootprint);
                            Debug.DrawLine(thisPos, neighboorPos);
                        }
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.A))
                container.SetActive(!container.activeSelf);
        }

        private GameObject GetElementFromNoise(float result)
        {
            float rule3 = ((buildings.Length - 1) / 10f) * result;

            int currentIndex = Mathf.RoundToInt(rule3);
            return buildings[currentIndex];
        }
    }
}