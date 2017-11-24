using UnityEngine;

namespace CityGenerator
{
    public class Apple
    {
        public Vector2 size { get; private set; }
        public Lot[] appleLots { get; private set; }

        //game object que voy a devolver, este va a contener a sus amados hijos
        private GameObject _objPadre = null;

        private static int _nPadre = 0; //para ir contando la cantidad de padres que genero
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridSize">tama de la manzana</param>
        /// <param name="_appleLots">todos los lots que hay en la manzada </param>
        public Apple(Vector2 gridSize, Lot[] _appleLots)
        {
            size = gridSize;
            appleLots = _appleLots;
        }

        public Apple(Lot[] _appleLots)
        {
            appleLots = _appleLots;
        }

        public GameObject BuildApple()
        {
            // esto ahora anda
            GameObject container = new GameObject(string.Format("Apple {0}/{1}", size.x, size.y));
            container.transform.position = Vector3.zero;
            foreach (Lot lot in appleLots)
            {
                GameObject lotGo = MonoBehaviour.Instantiate(lot.buildings[Random.Range(0, lot.buildings.Count - 1)],
                    lot.worldPos, Quaternion.identity);
                lotGo.transform.SetParent(container.transform);
            }
            return container;
        }
    }
}