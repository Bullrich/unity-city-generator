using UnityEngine;

namespace CityGenerator
{
    public class Apple : MonoBehaviour
    {
        public Vector2 size { get; private set; }
        public Lot[] appleLots { get; private set; }

        //game object que voy a devolver, este va a contener a sus amados hijos
        private GameObject _objPadre = null;

        private static int _nPadre = 0; //para ir contando la cantidad de padres que genero

        //ángulo de rotación de los edificios
        private const int C_ANGULO1=1;
        private const int C_ANGULO2 =2;
        private const int C_ANGULO3 = 3;
        private const int C_ANGULO4 = 4;

        

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

                    GameObject auxObj = UnityEngine.GameObject.Instantiate(appleLots[I].buildings[Random.Range(0, appleLots[I].buildings.Count - 1)],
                            appleLots[I].worldPos, Quaternion.identity); //le pongo el cubo de la grilla en el gameobject auxiliar

                    int DirRotacion = Random.Range(1, 5); //agrego una rotación a los edificios para hacer mas random el diseño
                    float angulin=0;
                    switch (DirRotacion)
                    {
                        case C_ANGULO1:
                            angulin = 0f;
                            break;

                        case C_ANGULO2:
                            angulin = 90f;
                            break;

                        case C_ANGULO3:
                            angulin = 180f;
                            break;

                        case C_ANGULO4:
                            angulin = 270f;
                            //angulin = 45f;
                            break;
                    }
                    auxObj.transform.Rotate(0f, angulin, 0f);

                    auxObj.transform.SetParent(_objPadre.transform);
                }

                return _objPadre;

            }
            catch(System.Exception Err)
            {
                Debug.Log("ERROR ERROR Y ERROR");
                Debug.Log(Err.Message);
                return null;
            }
            

        }
    }
}