using System;
using UnityEngine;

namespace Equipment
{
    public class Weapon : Item
    {
        public int power;
        public int range;
        public Sprite attackPrefab;
        public bool isRanged;
    }
}