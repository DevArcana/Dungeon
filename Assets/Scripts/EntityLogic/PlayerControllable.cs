using System;
using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using UnityEngine;
using World.Common;
using World.Interaction;

namespace EntityLogic
{
    [RequireComponent(typeof(PlayerEntity))]
    public class PlayerControllable : MonoBehaviour
    {

        private PlayerEntity _player;
        private void Start()
        {
            _player = GetComponent<PlayerEntity>();
            Highlight();
        }

        private class Tile
        {
            public GridPos gridPos;
            public byte height;
            public int cost;

            public Tile(GridPos gridPos, byte height, int cost)
            {
                this.gridPos = gridPos;
                this.height = height;
                this.cost = cost;
            }
        }
        
        private void Highlight()
        {
            var s = SelectionManager.instance;
            var world = World.World.instance;
            
            s.Clear();
            
            var playerPos = _player.GridPos;
            
            var queue = new Queue<GridPos>();
            queue.Enqueue(playerPos);

            var tiles = new Dictionary<GridPos, Tile>();
            var unavailableTiles = new List<GridPos> { playerPos };

            while (queue.Any())
            {
                var tile = queue.Dequeue();
                var cost = tiles.ContainsKey(tile) ? tiles[tile].cost : 0;
                var height = world.GetHeightAt(tile);
                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        var pos = GridPos.At(x + tile.x, y + tile.y);
                        if (unavailableTiles.Contains(pos)) continue;
                        if (world.GetEntities(pos).Any())
                        {
                            unavailableTiles.Add(pos);
                            continue;
                        }
                        var h = world.GetHeightAt(pos);
                        var dif = Math.Abs(height - h);
                        if (dif > 1) continue;
                        Tile t;
                        if (tiles.ContainsKey(pos))
                        {
                            t = tiles[pos];
                        }
                        else
                        {
                            t = new Tile(pos, h, cost + dif + 1);
                            queue.Enqueue(pos);
                        }
                        if (t.cost > cost + dif + 1)
                        {
                            t.cost = cost + dif + 1;
                            queue.Enqueue(pos);
                        }

                        if (t.cost > 5) continue;
                        tiles[pos] = t;
                    }
                }
            }

            foreach (var tile in tiles.Values)
            {
                s.Highlight(tile.gridPos);
            }
        }
    }
}