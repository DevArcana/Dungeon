using System;
using UnityEngine;

namespace Equipment
{
    [Serializable]
    public class Weapon
    {
        public int power;
        public int range = 20;
        public Sprite attackPrefab;
        public bool isRanged;
    }
}