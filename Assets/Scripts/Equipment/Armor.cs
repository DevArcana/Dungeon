using System;
using UnityEngine;

namespace Equipment
{
    [Serializable]
    public class Armor : Item
    {
        public int physicalDefense;
        public int magicalDefense;
    }
}