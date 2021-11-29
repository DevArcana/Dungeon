using System.Collections.Generic;
using EntityLogic;
using Equipment;
using UnityEngine;

namespace World.Triggers
{
    public class LootBox : GridTriggerEntity
    {
        public List<Item> items;

        public override void OnTileEntered(GridLivingEntity entity)
        {
            if (entity is null || !(entity is PlayerEntity))
            {
                return;
            }
            entity.equipment.backpack.AddRange(items);
            Destroy(gameObject);
        }
    }
}