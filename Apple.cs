using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityGenerator
{
    public class Apple
    {
        public Vector2 size { get; private set; }
        public Lot[] appleLots { get; private set; }

        private GameObject _objPadre = null;

        //para ir contando la cantidad de padres que genero
        private static int _nPadre = 0; 

        public Apple(Vector2 gridSize, Lot[] _appleLots)
        {
            //poner nombres a los gameobject padre
            size = gridSize;
            appleLots = _appleLots;
            Debug.Log(size);
            Debug.Log(new Vector3(size.x * BuildCity.buildingFootprint, 0, size.y * BuildCity.buildingFootprint));
        }

        public GameObject BuildApple()
        {
            try
            {
                _objPadre = new GameObject("Parent_" + _nPadre);
                _nPadre++;

                for (int I = 0; I < appleLots.Length; I++)
                {

                    if (I == 0) //seteo el gameobject padre en la posición del primer cubo de la manzana
                    {
                        _objPadre.transform.position = appleLots[I].worldPos;
                    }
                    //le pongo el cubo de la grilla en el gameobject auxiliar
                    GameObject auxObj = UnityEngine.GameObject.Instantiate(appleLots[I].buildings[Random.Range(0, appleLots[I].buildings.Length)], appleLots[I].worldPos, Quaternion.identity);
                    auxObj.name = _nPadre.ToString();
                    auxObj.transform.SetParent(_objPadre.transform);



                    //auxObj.transform.SetParent(_objPadre.transform);
                }

                // generate apple based in the size and store all the apple under a gameobject. Then return such gameobject
                //return null;
                return _objPadre;
            }
            catch (System.Exception Err)
            {
                Debug.Log(Err.Message);
                return null;
            }
        }
    }
}