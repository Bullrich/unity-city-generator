using System.Collections;
using System.Collections.Generic;
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
            Debug.ClearDeveloperConsole();
            Debug.Log("Valor size:" + size);
            Debug.Log("Distancia: " + new Vector3(size.x * BuildCity.buildingFootprint, 0, size.y * BuildCity.buildingFootprint));
        }

        public Apple(Lot[] _appleLots)
        {
            appleLots = _appleLots;
        }

        public GameObject BuildApple()
        {
            try
            {
                _objPadre = new GameObject("Parent_" + _nPadre);
                _nPadre++;

                for (int I=0;I<appleLots.Length;I++)
                {

                    if (I == 0) //seteo el gameobject padre en la posición del primer cubo de la manzana
                    {
                        _objPadre.transform.position = appleLots[I].worldPos;
                    }
                    //le pongo el cubo de la grilla en el gameobject auxiliar
                    GameObject auxObj = UnityEngine.GameObject.Instantiate(appleLots[I].buildings[Random.Range(0, appleLots[I].buildings.Length)], appleLots[I].worldPos, Quaternion.identity); 
                    //GameObject auxObj = UnityEngine.GameObject.Instantiate(appleLots[I].buildings[0], appleLots[I].worldPos, Quaternion.identity);
                    auxObj.transform.SetParent(_objPadre.transform);
                }

                // generate apple based in the size and store all the apple under a gameobject. Then return such gameobject
                //return null;
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