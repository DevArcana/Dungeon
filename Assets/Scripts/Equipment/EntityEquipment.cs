using System;
using System.Collections.Generic;
using UnityEngine;

namespace Equipment
{
    public class EntityEquipment : MonoBehaviour
    {
        public bool isEnabled;
        public GameObject inventory;
        private int _numberOfSlots;
        private GameObject[] _slots;
        public GameObject slotHolder;
        
        public Weapon weapon;
        public Helmet helmet;
        public Breastplate breastplate;
        public Leggings leggings;
        public Boots boots;
        public Necklace necklace;
        public Ring leftRing;
        public Ring rightRing;
        public Gloves gloves;
        public List<Item> backpack;


        private void Start()
        {
            _numberOfSlots = 28;
            _slots = new GameObject[_numberOfSlots];

            for (var i = 0; i < _numberOfSlots; i++)
            {
                _slots[i] = slotHolder.transform.GetChild(i).gameObject;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                isEnabled = !isEnabled;
            }

            inventory.SetActive(isEnabled);
        }
    }
}