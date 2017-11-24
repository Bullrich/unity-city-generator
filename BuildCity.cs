using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        public int seed = 0;
        public bool randomSeed;
        public bool randomStreets;

        private Vector3 worldSize;

        public static int buildingFootprint
        {
            get { return 3; }
        }

        public Neighborhood[] neighborhoods;

        private GameObject container;

        private ILot[,] map;

        private List<Apple> apples = new List<Apple>();

        private void Start()
        {
            worldSize = WorldSize();
            GenerateMap();
        }

        private Vector3 WorldSize()
        {
            return new Vector3((mapWidth * buildingFootprint), 0, (mapHeight * buildingFootprint));
        }

        private void GenerateMap()
        {
            container = new GameObject("CityContainer");
            // we set the seed
            if (randomSeed)
                seed = Random.Range(0, 400);
            Random.InitState(seed);

            map = GenerateGrid();

            CalculateCityLots(map);
            InstantiateStreets(map);

            for (int I=0;I<apples.Count; I++)
            {
                apples[I].manzana.transform.SetParent(container.transform);
            }

            // we restore true randomess
            Random.InitState(System.Environment.TickCount);
        }

        private ILot[,] GenerateGrid()
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

            // get cross streets
            if (randomStreets)
                for (x = 0; x < grid.GetLength(0); x++)
                {
                    for (z = 0; z < grid.GetLength(1); z++)
                    {
                        if ((grid[x, z] != null && grid[x, z].lotType == LotType.Street) &&
                            canBuildRandomStreet(grid, x, z) && Random.Range(0, 10) > 8)
                        {
                            while ((++z < grid.GetLength(1)) &&
                                   (grid[x, z] == null || grid[x, z].lotType != LotType.Street))
                            {
                                grid[x, z] = new Street(xStreets, new Vector2(x, z));
                            }
                        }
                    }
                }

            // add buildings
            float perlinSeed = Random.Range(0f, 40f);

            for (int h = 0; h < mapHeight; h++)
            {
                for (int w = 0; w < mapWidth; w++)
                {
                    if (grid[w, h] == null || grid[w, h].lotType != LotType.Street)
                    {
                        float perlin = Mathf.PerlinNoise(w / 10f + perlinSeed, h / 10f + perlinSeed) * 10;
                        grid[w, h] = new Lot(new Vector2(w, h), GetNeighborhoodFromNoise(perlin));
                    }
                    grid[w, h].worldPos = new Vector3(w * buildingFootprint, 0, h * buildingFootprint);
                }
            }

            return grid;
        }

        private bool canBuildRandomStreet(ILot[,] grid, int x, int z)
        {
            if (x <= 0 || x >= grid.GetLength(0) - 1 ||z <= 0 ||z >= grid.GetLength(1) - 1)
                return false;
    
            ILot[] lots = new ILot[] {grid[x +1, z + 1], grid[x-1, z + 1]};
            foreach (ILot lot in lots)
            {
                if (lot != null && lot.lotType == LotType.Street)
                    return false;
            }
            return true;
        }

        private void InstantiateGridElements(ILot[,] lots)
        {
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

        private void InstantiateStreets(ILot[,] lots)
        {
            for (int i = 0; i < lots.GetLength(0); i++)
            {
                for (int j = 0; j < lots.GetLength(1); j++)
                {
                    //descomente esto para que solo cree las calles, los edificios se crean en el objeto Apple.
                    if (lots[i, j].lotType == LotType.Street) 
                    {
                        GameObject street =
                            Instantiate(lots[i, j].buildings[0], lots[i, j].worldPos,
                                lots[i, j].buildings[0].transform.rotation);
                        street.transform.SetParent(container.transform);
                    }
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
                            newApple.BuildApple();//creo las manzanas
                            apples.Add(newApple); 
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
                apples.Clear();
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
            if (neighborhoods.Length > 0)
            {
                int totalChance = 0;
                foreach (Neighborhood ng in neighborhoods)
                {
                    totalChance += ng.ChanceToAppear;
                }
                // Usamos regla de tres simple con ruleta no se que cosa para permitir que se decida las chances 
                // de que aparezca algo en base al porcentaje que el usuario le dio.
                float newChance = (totalChance / 10f) * result;
                int currentChances = 0;
                for (int i = 0; i < neighborhoods.Length; i++)
                {
                    currentChances += neighborhoods[i].ChanceToAppear;

                    if (currentChances > newChance)
                        return neighborhoods[i];
                }
            }
            return new Neighborhood();
        }
    }
}