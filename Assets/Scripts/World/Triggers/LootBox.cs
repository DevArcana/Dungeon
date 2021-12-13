using System;
using System.Collections.Generic;
using EntityLogic;
using EntityLogic.AI;
using Equipment;
using UnityEngine;

namespace World.Triggers
{
    public class LootBox : GridTriggerEntity
    {
        public List<Item> items;

        private void Start()
        {
            entityName = "LootBox";
        }

        public override void OnTileEntered(GridLivingEntity entity)
        {
            if (entity is null || !(entity is PlayerEntity))
            {
                return;
            }

            var availableSlots = entity.equipment.backpack.Capacity - entity.equipment.backpack.Count;
            if (availableSlots > items.Count)
            {
                entity.equipment.backpack.AddRange(items);
                foreach (var item in items)
                {
                    LogConsole.Log($"Picked up: {item.itemName} ({item.itemRarity})" + Environment.NewLine);
                }
                Destroy(gameObject);
            }
        }

        public override string GetTooltip()
        {
            return "LootBox";
        }
    }
}