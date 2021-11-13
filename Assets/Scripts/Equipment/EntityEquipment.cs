using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
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

        public GameObject itemDescription;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDescriptionText;
        public Image icon;
        public GameObject useButton;
        public static bool isItemDescriptionEnabled;
        private static bool _isItemConsumable;

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
            _isItemConsumable = false;
            isItemDescriptionEnabled = false;
            _numberOfSlots = 39;
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
                isItemDescriptionEnabled = false;

            }
            if (isEnabled && !iconsGenerated)
            {
                for (var i = 0; i < backpack.Count; i++)
                {
                    _slots[i].GetComponent<Image>().sprite = backpack[i].icon;
                    var x = i;
                    _slots[i].GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(x));
                }

                iconsGenerated = true;
            }
            useButton.SetActive(_isItemConsumable);
            itemDescription.SetActive(isItemDescriptionEnabled);
            inventory.SetActive(isEnabled);
        }

        private void OnItemClicked(int index)
        {
            isItemDescriptionEnabled = true;
            itemName.text = backpack[index].itemName;
            itemDescriptionText.text = backpack[index].description;
            icon.sprite = backpack[index].icon;
            if (backpack[index] is Consumable)
            {
                _isItemConsumable = true;
                useButton.GetComponent<Button>().onClick.AddListener(((Consumable)backpack[index]).Use);
            }
            else
            {
                _isItemConsumable = false;
            }
        }
    }
}