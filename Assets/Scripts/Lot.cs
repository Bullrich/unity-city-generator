using UnityEngine;

namespace CityGenerator
{
    public class Lot
    {
        public bool street;
        public Vector2 gridPos;
        public Vector3 worldPos;
        public float lotType;
        public Lot neighrboor;

        public Lot(bool _street, Vector2 _gridPos, float _type)
        {
            street = _street;
            gridPos = _gridPos;
            lotType = _type;
        }
    }
}