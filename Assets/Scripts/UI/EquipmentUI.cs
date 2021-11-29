using EntityLogic;
using Equipment;
using TMPro;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EquipmentUI : MonoBehaviour
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
        public TextMeshProUGUI itemAttributesNamesText;
        public TextMeshProUGUI itemAttributesValuesText;
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

        private EntityEquipment _equipment;

        public TextMeshProUGUI attributeNames;
        public TextMeshProUGUI attributeValues;
        private void Start()
        {
            iconsGenerated = false;
            isEnabled = false;
            isItemDescriptionEnabled = false;
            _numberOfSlots = 39;
            _slots = new GameObject[_numberOfSlots];

            for (var i = 0; i < _numberOfSlots; i++)
            {
                _slots[i] = slotHolder.transform.GetChild(i).gameObject;
            }
        }

        private void Update()
        {
            if (!(TurnManager.instance.CurrentTurnTaker is PlayerEntity)) return;
            if (CraftingUI.IsEditingInputField) return;
            _equipment = TurnManager.instance.CurrentTurnTaker.equipment;
            if (Input.GetKeyDown(KeyCode.I))
            {
                isEnabled = !isEnabled;
                iconsGenerated = false;
                isItemDescriptionEnabled = false;
                CraftingUI.isCraftingEnabled = false;
            }
            if (isEnabled && !iconsGenerated)
            {
                ShowStatistics();
                for (var i = 0; i < _equipment.backpack.Count; i++)
                {
                    _slots[i].GetComponent<Image>().sprite = _equipment.backpack[i].icon;
                    var x = i;
                    _slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    _slots[i].GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.backpack[x], false));
                }

                for (var i = _equipment.backpack.Count; i < _numberOfSlots; i++)
                {
                    _slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    _slots[i].GetComponent<Image>().sprite = background;
                }
                if (!(_equipment.weapon is null))
                {
                    weaponSlot.GetComponent<Image>().sprite = _equipment.weapon.icon;
                    weaponSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    weaponSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.weapon));
                    weaponSlot.SetActive(true);
                }
                else
                {
                    weaponSlot.SetActive(false);
                }
                if (!(_equipment.helmet is null))
                {
                    helmetSlot.GetComponent<Image>().sprite = _equipment.helmet.icon;
                    helmetSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    helmetSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.helmet));
                    helmetSlot.SetActive(true);
                }
                else
                {
                    helmetSlot.SetActive(false);
                }
                if (!(_equipment.breastplate is null))
                {
                    breastplateSlot.GetComponent<Image>().sprite = _equipment.breastplate.icon;
                    breastplateSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    breastplateSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.breastplate));
                    breastplateSlot.SetActive(true);
                }
                else
                {
                    breastplateSlot.SetActive(false);
                }
                if (!(_equipment.leggings is null))
                {
                    leggingsSlot.GetComponent<Image>().sprite = _equipment.leggings.icon;
                    leggingsSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    leggingsSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.leggings));
                    leggingsSlot.SetActive(true);
                }
                else
                {
                    leggingsSlot.SetActive(false);
                }
                if (!(_equipment.boots is null))
                {
                    bootsSlot.GetComponent<Image>().sprite = _equipment.boots.icon;
                    bootsSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    bootsSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.boots));
                    bootsSlot.SetActive(true);
                }
                else
                {
                    bootsSlot.SetActive(false);
                }
                if (!(_equipment.necklace is null))
                {
                    necklaceSlot.GetComponent<Image>().sprite = _equipment.necklace.icon;
                    necklaceSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    necklaceSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.necklace));
                    necklaceSlot.SetActive(true);
                }
                else
                {
                    necklaceSlot.SetActive(false);
                }
                if (!(_equipment.ring is null))
                {
                    ringSlot.GetComponent<Image>().sprite = _equipment.ring.icon;
                    ringSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    ringSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.ring));
                    ringSlot.SetActive(true);
                }
                else
                {
                    ringSlot.SetActive(false);
                }
                if (!(_equipment.gloves is null))
                {
                    glovesSlot.GetComponent<Image>().sprite = _equipment.gloves.icon;
                    glovesSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                    glovesSlot.GetComponent<Button>().onClick.AddListener(()=>OnItemClicked(_equipment.gloves));
                    glovesSlot.SetActive(true);
                }
                else
                {
                    glovesSlot.SetActive(false);
                }
                iconsGenerated = true;
            }
            itemDescription.SetActive(isItemDescriptionEnabled);
            MakeVisible(isEnabled);
        }

        private void OnItemClicked(Item item, bool isEquipped = true)
        {
            isItemDescriptionEnabled = true;
            itemName.text = item.itemName + " (" + item.itemRarity + ")";
            itemDescriptionText.text = item.description;
            icon.sprite = item.icon;
            if (item is Consumable consumable)
            {
                useButton.onClick.RemoveAllListeners();
                useButton.onClick.AddListener(consumable.Use);
                useButton.onClick.AddListener(() =>
                {
                    _equipment.backpack.Remove(consumable);
                    iconsGenerated = false;
                    isItemDescriptionEnabled = false;
                });
                
                buttonText.text = "USE";
            }
            else
            {
                if (isEquipped)
                {
                    buttonText.text = "UNEQUIP";
                    useButton.onClick.RemoveAllListeners();
                    useButton.onClick.AddListener(()=> TurnManager.instance.Transactions.EnqueueTransaction( new UnEquipItemTransaction(TurnManager.instance.CurrentTurnTaker, item, false)));
                }
                else
                {
                    buttonText.text = "EQUIP";
                    useButton.onClick.RemoveAllListeners();
                    useButton.onClick.AddListener(() => TurnManager.instance.Transactions.EnqueueTransaction( new EquipItemTransaction(TurnManager.instance.CurrentTurnTaker, item, false)));
                }
            }
            removeButton.onClick.RemoveAllListeners();
            removeButton.onClick.AddListener(() => TurnManager.instance.Transactions.EnqueueTransaction( new RemoveItemTransaction(TurnManager.instance.CurrentTurnTaker, item, isEquipped, false)));

            itemAttributesNamesText.text = item.AttributeNames();
            itemAttributesValuesText.text = item.AttributeValues();
            useButton.interactable = !(item is WeaponComponent);
        }

        private void MakeVisible(bool isEnabled)
        {
            inventory.transform.localScale = isEnabled ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        }

        private void ShowStatistics()
        {
            var player = TurnManager.instance.CurrentTurnTaker;
            player.RecalculateAttributes();
            attributeNames.text = player.attributes.AttributeNames();
            attributeValues.text = player.attributes.AttributeValues() + player.health.MaximumHealth + "\n" + player.health.Health;
        }
    }
}