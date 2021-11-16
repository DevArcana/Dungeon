using System.Collections.Generic;
using TMPro;
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

        public GameObject itemDescription;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDescriptionText;
        public Image icon;
        public GameObject useButton;
        public static bool isItemDescriptionEnabled;
        private static bool _isItemConsumable;

        public GameObject weaponSlot;
        public GameObject helmetSlot;
        public GameObject breastplateSlot;
        public GameObject leggingsSlot;
        public GameObject bootsSlot;
        public GameObject necklaceSlot;
        public GameObject ringSlot;
        public GameObject glovesSlot;

        public Weapon weapon;
        public Helmet helmet;
        public Breastplate breastplate;
        public Leggings leggings;
        public Boots boots;
        public Necklace necklace;
        public Ring ring;
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
                    _slots[i].GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(backpack[x]));
                    
                }
                if (!(weapon is null))
                {
                    weaponSlot.GetComponent<Image>().sprite = weapon.icon;
                    weaponSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(weapon));
                    weaponSlot.SetActive(true);
                }
                else
                {
                    weaponSlot.SetActive(false);
                }
                if (!(helmet is null))
                {
                    helmetSlot.GetComponent<Image>().sprite = helmet.icon;
                    helmetSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(helmet));
                    helmetSlot.SetActive(true);
                }
                else
                {
                    helmetSlot.SetActive(false);
                }
                if (!(breastplate is null))
                {
                    breastplateSlot.GetComponent<Image>().sprite = breastplate.icon;
                    breastplateSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(breastplate));
                    breastplateSlot.SetActive(true);
                }
                else
                {
                    breastplateSlot.SetActive(false);
                }
                if (!(leggings is null))
                {
                    leggingsSlot.GetComponent<Image>().sprite = leggings.icon;
                    leggingsSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(leggings));
                    leggingsSlot.SetActive(true);
                }
                else
                {
                    leggingsSlot.SetActive(false);
                }
                if (!(boots is null))
                {
                    bootsSlot.GetComponent<Image>().sprite = boots.icon;
                    bootsSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(boots));
                    bootsSlot.SetActive(true);
                }
                else
                {
                    bootsSlot.SetActive(false);
                }
                if (!(necklace is null))
                {
                    necklaceSlot.GetComponent<Image>().sprite = necklace.icon;
                    necklaceSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(necklace));
                    necklaceSlot.SetActive(true);
                }
                else
                {
                    necklaceSlot.SetActive(false);
                }
                if (!(ring is null))
                {
                    ringSlot.GetComponent<Image>().sprite = ring.icon;
                    ringSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(ring));
                    ringSlot.SetActive(true);
                }
                else
                {
                    ringSlot.SetActive(false);
                }
                if (!(gloves is null))
                {
                    glovesSlot.GetComponent<Image>().sprite = gloves.icon;
                    glovesSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(gloves));
                    glovesSlot.SetActive(true);
                }
                else
                {
                    glovesSlot.SetActive(false);
                }
                iconsGenerated = true;
            }
            useButton.SetActive(_isItemConsumable);
            itemDescription.SetActive(isItemDescriptionEnabled);
            inventory.SetActive(isEnabled);
        }

        private void OnItemClicked(Item item)
        {
            isItemDescriptionEnabled = true;
            itemName.text = item.itemName;
            itemDescriptionText.text = item.description;
            icon.sprite = item.icon;
            if (item is Consumable consumable)
            {
                _isItemConsumable = true;
                useButton.GetComponent<Button>().onClick.AddListener(consumable.Use);
            }
            else
            {
                _isItemConsumable = false;
            }
        }
        
    }
}