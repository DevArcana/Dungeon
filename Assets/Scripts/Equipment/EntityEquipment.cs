using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Equipment
{
    public class EntityEquipment : MonoBehaviour
    {
        public static bool isEnabled;
        public static bool iconsGenerated;
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
            iconsGenerated = false;
            isEnabled = false;
            _numberOfSlots = 28;
            backpack.Capacity = _numberOfSlots;
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
                iconsGenerated = false;

            }
            if (isEnabled && !iconsGenerated)
            {
                for (var i = 0; i < backpack.Count; i++)
                {
                    _slots[i].GetComponent<Image>().sprite = backpack[i].icon;
                }

                iconsGenerated = true;
            }

            inventory.SetActive(isEnabled);
        }
    }
}