using System;
using Equipment;
using TurnSystem;
using UnityEngine;

namespace UI
{
    public class CraftingUI : MonoBehaviour
    {
        public GameObject crafting;
        private EntityEquipment _equipment;
        public static bool isCraftingEnabled;

        private void Start()
        {
            isCraftingEnabled = false;
        }

        private void Update()
        {
            if (!(TurnManager.instance.CurrentTurnTaker is PlayerEntity)) return;
            _equipment = TurnManager.instance.CurrentTurnTaker.equipment;
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCraftingEnabled = !isCraftingEnabled;
                EquipmentUI.isEnabled = false;
            }
            MakeVisible(isCraftingEnabled);
        }
        
        private void MakeVisible(bool enabled)
        {
            if (enabled)
            {
                crafting.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                crafting.transform.localScale = new Vector3(0, 0, 0);
            }
        }
    }
}