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
        }

        public GameObject BuildApple()
        {
            // esto ahora anda
            GameObject container = new GameObject(string.Format("Apple {0}/{1}", size.x, size.y));
            container.transform.position = Vector3.zero;
            foreach (Lot lot in appleLots)
            {
                GameObject lotGo = MonoBehaviour.Instantiate(lot.buildings[Random.Range(0, lot.buildings.Length - 1)],
                    lot.worldPos, Quaternion.identity);
                lotGo.transform.SetParent(container.transform);
            }
            return container;
        }
    }
}