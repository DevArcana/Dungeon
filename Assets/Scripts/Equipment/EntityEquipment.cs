using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Equipment
{
    public class EntityEquipment : MonoBehaviour
    {
        public static bool isEnabled;
        public static bool iconsGenerated;
        public GameObject inventory;
        public Sprite background;
        
        private int _numberOfSlots;
        private GameObject[] _slots;
        public GameObject slotHolder;

        public GameObject itemDescription;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDescriptionText;
        public Image icon;
        public Button useButton;
        public TextMeshProUGUI buttonText;
        public static bool isItemDescriptionEnabled;
        public Button removeButton;

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
                    _slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    _slots[i].GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(backpack[x], false));
                }

                for (var i = backpack.Count; i < backpack.Capacity; i++)
                {
                    _slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    _slots[i].GetComponent<Image>().sprite = background;
                }
                if (!(weapon is null))
                {
                    weaponSlot.GetComponent<Image>().sprite = weapon.icon;
                    weaponSlot.GetComponent<Button>().onClick.RemoveAllListeners();
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
                    helmetSlot.GetComponent<Button>().onClick.RemoveAllListeners();
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
                    breastplateSlot.GetComponent<Button>().onClick.RemoveAllListeners();
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
                    leggingsSlot.GetComponent<Button>().onClick.RemoveAllListeners();
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
                    bootsSlot.GetComponent<Button>().onClick.RemoveAllListeners();
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
                    necklaceSlot.GetComponent<Button>().onClick.RemoveAllListeners();
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
                    ringSlot.GetComponent<Button>().onClick.RemoveAllListeners();
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
                    glovesSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    glovesSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(gloves));
                    glovesSlot.SetActive(true);
                }
                else
                {
                    glovesSlot.SetActive(false);
                }
                iconsGenerated = true;
            }
            itemDescription.SetActive(isItemDescriptionEnabled);
            inventory.SetActive(isEnabled);
        }

        private void OnItemClicked(Item item, bool isEquiped = true)
        {
            isItemDescriptionEnabled = true;
            itemName.text = item.itemName;
            itemDescriptionText.text = item.description;
            icon.sprite = item.icon;
            if (item is Consumable consumable)
            {
                useButton.onClick.RemoveAllListeners();
                useButton.onClick.AddListener(consumable.Use);
                useButton.onClick.AddListener(() =>
                {
                    backpack.Remove(consumable);
                    iconsGenerated = false;
                    isItemDescriptionEnabled = false;
                });
                
                buttonText.text = "USE";
            }
            else
            {
                if (isEquiped)
                {
                    buttonText.text = "UNEQUIP";
                    useButton.onClick.RemoveAllListeners();
                    useButton.onClick.AddListener(()=> UnEquip(item));
                }
                else
                {
                    buttonText.text = "EQUIP";
                    useButton.onClick.RemoveAllListeners();
                    useButton.onClick.AddListener(() => Equip(item));
                }
            }
            removeButton.onClick.RemoveAllListeners();
            removeButton.onClick.AddListener(() => RemoveItem(item, isEquiped));
        }

        private void Equip(Item item)
        {
            switch (item)
            {
                case Weapon w:
                {
                    if(!(weapon is null)) backpack.Add(weapon);
                    weapon = w;
                    backpack.Remove(w);
                    break;
                }
                case Helmet w:
                {
                    if(!(helmet is null)) backpack.Add(helmet);
                    helmet = w;
                    backpack.Remove(w);
                    break;
                }
                case Breastplate w:
                {
                    if(!(breastplate is null)) backpack.Add(breastplate);
                    breastplate = w;
                    backpack.Remove(w);
                    break;
                }
                case Leggings w:
                {
                    if(!(leggings is null)) backpack.Add(leggings);
                    leggings = w;
                    backpack.Remove(w);
                    break;
                }
                case Gloves w:
                {
                    if(!(gloves is null)) backpack.Add(gloves);
                    gloves = w;
                    backpack.Remove(w);
                    break;
                }
                case Boots w:
                {
                    if(!(boots is null)) backpack.Add(boots);
                    boots = w;
                    backpack.Remove(w);
                    break;
                }
                case Ring w:
                {
                    if(!(ring is null)) backpack.Add(ring);
                    ring = w;
                    backpack.Remove(w);
                    break;
                }
                case Necklace w:
                {
                    if(!(necklace is null)) backpack.Add(necklace);
                    necklace = w;
                    backpack.Remove(w);
                    break;
                }
            }

            iconsGenerated = false;
            isItemDescriptionEnabled = false;
        }
        
        private void UnEquip(Item item)
        {
            switch (item)
            {
                case Weapon w:
                {
                    backpack.Add(weapon);
                    weapon = null;
                    break;
                }
                case Helmet w:
                {
                    backpack.Add(helmet);
                    helmet = null;
                    break;
                }
                case Breastplate w:
                {
                    backpack.Add(breastplate);
                    breastplate = null;
                    break;
                }
                case Leggings w:
                {
                    backpack.Add(leggings);
                    leggings = null;
                    break;
                }
                case Gloves w:
                {
                    backpack.Add(gloves);
                    gloves = null;
                    break;
                }
                case Boots w:
                {
                    backpack.Add(boots);
                    boots = null;
                    break;
                }
                case Ring w:
                {
                    backpack.Add(ring);
                    ring = null;
                    break;
                }
                case Necklace w:
                {
                    backpack.Add(necklace);
                    necklace = null;
                    break;
                }
            }
            iconsGenerated = false;
            isItemDescriptionEnabled = false;
        }

        private void RemoveItem(Item item, bool isEquiped)
        {
            if (isEquiped)
            {
                UnEquip(item);
            }

            backpack.Remove(item);
            iconsGenerated = false;
            isItemDescriptionEnabled = false;
        }

    }
}