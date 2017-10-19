using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityGenerator
{
    public interface IApple
    {
        Vector2 Size { get; set; }
        void BuildApple(Lot[] apple);
    }
}