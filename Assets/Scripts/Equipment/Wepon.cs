using System;
using UnityEngine;

namespace Equipment
{
    [Serializable]
    public class Weapon : Item
    {
        public int power;
        public int range;
        public Sprite attackPrefab;
        public bool isRanged;
    }
}