using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// By @Bullrich
namespace CityGenerator
{
    public class BuildCity : MonoBehaviour
    {
        public GameObject
            xStreets,
            zStreets,
            crossRoad;

        public int
            mapWidth = 20,
            mapHeight = 20;

        private Vector3 worldSize;

        public static int buildingFootprint {get { return 3; }}
        //[SerializeField] private Neighborhood[] _neighborhoods;
        public Neighborhood[] neighborhoods;

        private GameObject container;

        private ILot[,] map;

        private List<Apple> apples = new List<Apple>();

        private void Start()
        {
            worldSize = WorldSize();
            GenerateMap();
        }

//        private void OnDrawGizmosSelected()
//        {
////            Vector3 size = WorldSize();
////            Gizmos.DrawWireCube(Vector3.zero, size);
//            
//        }

        private Vector3 WorldSize()
        {
            return new Vector3((mapWidth * buildingFootprint), 0, (mapHeight * buildingFootprint));
        }

        private void GenerateMap()
        {
            map = GenerateGrid(Random.Range(0, 50));

            // this should be done automaticaly by the Apple "build" command
            InstantiateGridElements(map);
            foreach (Apple apple in apples)
            {
                apple.BuildApple().transform.SetParent(container.transform);
            }

            CalculateCityLots(map);
        }

        private ILot[,] GenerateGrid(float seed)
        {
            ILot[,] grid = new ILot[mapWidth, mapHeight];

            // build streets
            int x = 0;
            for (int n = 0; n < 50; n++)
            {
                for (int h = 0; h < mapHeight; h++)
                {
                    grid[x, h] = new Street(zStreets, new Vector2(x, h));
                }
                x += Random.Range(2, 15);
                if (x >= mapWidth) break;
            }

            int z = 0;
            for (int n = 0; n < 10; n++)
            {
                for (int w = 0; w < mapWidth; w++)
                {
                    if (grid[w, z] != null && grid[w, z].lotType == LotType.Street)
                        grid[w, z] = new Street(crossRoad, new Vector2(w, z));
                    else
                        grid[w, z] = new Street(xStreets, new Vector2(w, z));
                }
                z += Random.Range(2, 20);
                if (z >= mapHeight) break;
            }

            // add buildings
            for (int h = 0; h < mapHeight; h++)
            {
                for (int w = 0; w < mapWidth; w++)
                {
                    if (grid[w, h] == null || grid[w, h].lotType != LotType.Street)
                    {
                        float perlin = Mathf.PerlinNoise(w / 10f + seed, h / 10f + seed) * 10;
                        grid[w, h] = new Lot(new Vector2(w, h), GetNeighborhoodFromNoise(perlin));
                    }
                    grid[w, h].worldPos = new Vector3(w * buildingFootprint, 0, h * buildingFootprint);
                }
            }

            return grid;
        }

        private void InstantiateGridElements(ILot[,] lots)
        {
            container = new GameObject("CityContainer");
            container.transform.position = Vector3.zero;
            for (int x = 0; x < lots.GetLength(0); x++)
            {
                for (int z = 0; z < lots.GetLength(1); z++)
                {
                    ILot lot = lots[x, z];
                    GameObject lotGo;
                    if (lot.lotType == LotType.Street)
                        lotGo = Instantiate(lot.buildings[0], lot.worldPos, lot.buildings[0].transform.rotation);
                    else
                        lotGo = Instantiate(lot.buildings[Random.Range(0, lot.buildings.Length - 1)],
                            lot.worldPos, Quaternion.identity);
                    lotGo.transform.SetParent(container.transform);
                }
            }
        }

        private void CalculateCityLots(ILot[,] lots)
        {
            List<Lot> apple = new List<Lot>();
            for (int x = 0; x < lots.GetLength(0); x++)
            {
                for (int z = 0; z < lots.GetLength(1); z++)
                {
                    if (lots[x, z].lotType == LotType.Lot)
                    {
                        Lot lot = (Lot) lots[x, z];
                        if (lot.neighboor == null)
                        {
                            GenerateApple(lots, x, z, apple);
                            Apple newApple = new Apple(GetAppleSize(apple), apple.ToArray());
                            apple.Clear();
                        }
                    }
                }
            }
        }

        private Vector2 GetAppleSize(List<Lot> apple)
        {
            Vector2 currentSize = Vector2.zero;
            foreach (Lot lot in apple)
            {
                if (lot.gridPos.x > currentSize.x || lot.gridPos.y > currentSize.y)
                    currentSize = lot.gridPos;
            }
            return currentSize;
        }

        /// <summary>Be sure that the index you are sending is of a valid LOT!</summary>
        private void GenerateApple(ILot[,] lots, int x, int z, List<Lot> apple)
        {
            Lot thisLot = (Lot) lots[x, z];
            apple.Add(thisLot);

            for (int i = 0; i < 2; i++)
            {
                Vector2 lotIndex = new Vector2(i == 0 ? x + 1 : x, z + i);
                if (lots.GetLength(0) > lotIndex.x && lots.GetLength(1) > lotIndex.y)
                {
                    ILot neighboor = lots[(int) lotIndex.x, (int) lotIndex.y];
                    if (neighboor.lotType == LotType.Lot && ((Lot) neighboor).neighboor == null)
                    {
                        ((Lot) neighboor).neighboor = thisLot;
                        GenerateApple(lots, (int) lotIndex.x, (int) lotIndex.y, apple);
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Destroy(container);
                container = null;
                GC.Collect();
                GenerateMap();
            }
            else if (Input.GetKey(KeyCode.M))
            {
                DrawNeighboor();
            }
            if (Input.GetKeyDown(KeyCode.A))
                container.SetActive(!container.activeSelf);
        }

        /*
         * 26/10/17: Por si nos sirve para el input.
         * 
        public void Input_Generate()
        {
            Destroy(container);
            container = null;
            GC.Collect();
            GenerateMap();
        }

        public void Input_Draw()
        {
            DrawNeighboor();
        }

        public void Input_SetActive()
        {
            container.SetActive(!container.activeSelf);
        }
        */

        private void DrawNeighboor()
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int z = 0; z < map.GetLength(1); z++)
                {
                    ILot ilot = map[x, z];
                    if (ilot.lotType == LotType.Lot)
                    {
                        Lot lot = ((Lot) ilot);
                        if (lot.neighboor != null)
                        {
                            Vector3 thisPos = lot.worldPos;
                            Vector3 neighboorPos = ((Lot) lot).neighboor.worldPos;
                            Debug.DrawLine(thisPos, neighboorPos, Color.red);
                        }
                    }
                }
            }
        }

        private Neighborhood GetNeighborhoodFromNoise(float result)
        {
			if (neighborhoods.Length > 0) {
				//float rule3 = ((_neighborhoods.Length - 1) / 10f) * result;
				float rule3 = ((neighborhoods.Length - 1) / 10f) * result;

				int currentIndex = Mathf.RoundToInt (rule3);
				//return _neighborhoods[currentIndex];
				return neighborhoods [currentIndex];
			} else return new Neighborhood ();
        }
    }
}