using System;
using UnityEngine;

namespace CityGenerator
{
    public interface ILot
    {
        Vector2 gridPos { get; }
        Vector3 worldPos { get; set; }
        GameObject[] buildings { get; }
        LotType lotType { get; }
    }

    public class Lot : ILot
    {
        public Vector2 gridPos { get; private set; }
        public Vector3 worldPos { get; set; }
        public GameObject[] buildings { get; private set; }
        public Lot neighboor;

        public LotType lotType
        {
            get { return LotType.Lot; }
        }

        public Lot(Vector2 _gridPos, Neighborhood neighborhood)
        {
            gridPos = _gridPos;
            buildings = neighborhood.buildings;
        }
    }

    public class Street : ILot
    {
        public Vector2 gridPos { get; private set; }
        public Vector3 worldPos { get; set; }

        public GameObject[] buildings { get; private set; }

        public LotType lotType
        {
            get { return LotType.Street; }
        }

        public enum streetType
        {
            xStreet,
            zStreet,
            Crossroad
        }

        public streetType street { get; private set; }

        public Street(GameObject street, Vector2 _gridPos)
        {
            buildings = new GameObject[1] {street};
            gridPos = _gridPos;
        }
    }

    public enum LotType
    {
        Street,
        Lot
    }

    [Serializable]
    public class Neighborhood
    {
        public GameObject[] buildings;
        [Range(0, 100)] public int ChanceToAppear = 3;
    }
}