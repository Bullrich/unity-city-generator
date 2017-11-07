using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityGenerator
{
    public class Apple
    {
        public Vector2 size { get; private set; }
        public Lot[] appleLots { get; private set; }

        public Apple(Vector2 gridSize, Lot[] _appleLots)
        {
            size = gridSize;
            appleLots = _appleLots;
            Debug.Log(size);
            Debug.Log(new Vector3(size.x * BuildCity.buildingFootprint, 0, size.y * BuildCity.buildingFootprint));
        }

        public GameObject BuildApple()
        {
            // generate apple based in the size and store all the apple under a gameobject. Then return such gameobject
            return null;
        }
    }
}