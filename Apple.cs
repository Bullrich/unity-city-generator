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

        public GameObject manzana
        {
            get
            {
                return _objPadre;
            }
        }


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
            #region código javier
            // esto ahora anda
            /*
            GameObject container = new GameObject(string.Format("Apple {0}/{1}", size.x, size.y));
            container.transform.position = Vector3.zero;
            foreach (Lot lot in appleLots)
            {
                GameObject lotGo = MonoBehaviour.Instantiate(lot.buildings[Random.Range(0, lot.buildings.Length - 1)],
                    lot.worldPos, Quaternion.identity);
                lotGo.transform.SetParent(container.transform);
            }
            return container;
            */
            #endregion
            

            
             try
            {
                _objPadre = new GameObject("Parent_" + _nPadre + " " + string.Format("Apple {0}/{1}", size.x, size.y));
                _nPadre++;
                Debug.Log(appleLots.Length);

                for (int I=0;I<appleLots.Length;I++)
                {

                    if (I == 0) //seteo el gameobject padre en la posición del primer cubo de la manzana
                    {
                        _objPadre.transform.position = appleLots[I].worldPos;
                    }
                    GameObject auxObj = UnityEngine.GameObject.Instantiate(appleLots[I].buildings[Random.Range(0, appleLots[I].buildings.Length -1)],
                          appleLots[I].worldPos, Quaternion.identity); //le pongo el cubo de la grilla en el gameobject auxiliar
                    auxObj.transform.SetParent(_objPadre.transform);
                }

                return _objPadre;

            }
            catch(System.Exception Err)
            {
                Debug.Log(Err.Message);
                return null;
            }
            

        }
    }
}