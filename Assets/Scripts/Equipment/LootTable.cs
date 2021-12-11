using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Equipment
{
    [Serializable]
    public class LootDrop
    {
        public Item item;
        public int weight;
    }
    
    [Serializable]
    public class LootTable
    {
        public List<LootDrop> loot;
        public int numberOfItemDropped;

        public List<Item> GetDrop()
        {
            loot = loot.OrderBy(x => x.weight).ToList();
            var returnedLoot = new List<Item>();
            for (var i = 0; i < numberOfItemDropped; i++)
            {
                var roll = Random.Range(0, 101);
                var weightedSum = 0;
                foreach (var drop in loot)
                {
                    weightedSum += drop.weight;
                    if (roll < weightedSum)
                    {
                        returnedLoot.Add(drop.item);
                    }
                }
            }
            return returnedLoot;
        }
    }
}