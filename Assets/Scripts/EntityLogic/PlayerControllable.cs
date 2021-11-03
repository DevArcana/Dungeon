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

            TurnManager.instance.ActionPoints.ActionPointsChanged += OnActionPointsChanged;
            TurnManager.instance.TurnChanged += OnTurnChanged;
            Highlight();
        }

        private void OnDestroy()
        {
            TurnManager.instance.ActionPoints.ActionPointsChanged -= OnActionPointsChanged;
            TurnManager.instance.TurnChanged -= OnTurnChanged;
        }

        private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
        {
            Highlight();
        }

        private void OnActionPointsChanged(int points)
        {
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
            var turns = TurnManager.instance;

            s.ClearTargetPositions();
            if (turns.CurrentTurnTaker != _player)
            {
                return;
            }

            var pos = _player.GridPos;
            var cost = 0;
            var height = world.GetHeightAt(pos);

            var tiles = new Dictionary<GridPos, Tile>
            {
                [pos] = new Tile(pos, height, cost)
            };

            var queue = new Queue<GridPos>();
            queue.Enqueue(pos);
            
            while (queue.Any())
            {
                var tile = tiles[queue.Dequeue()];

                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        
                        pos = GridPos.At(x + tile.gridPos.x, y + tile.gridPos.y);
                        if (pos == _player.GridPos) continue;

                        if (tiles.ContainsKey(pos))
                        {
                            var neighbour = tiles[pos];
                            var heightDifference = Math.Abs(tile.height - neighbour.height);
                            if (heightDifference > 1) continue;

                            cost = tile.cost + heightDifference + 1;
                            if (cost < neighbour.cost)
                            {
                                neighbour.cost = cost;
                                if (neighbour.cost <= TurnManager.instance.ActionPoints.ActionPoints)
                                {
                                    queue.Enqueue(neighbour.gridPos);
                                }
                            }
                        }
                        else
                        {
                            height = world.GetHeightAt(pos);
                            var heightDifference = Math.Abs(tile.height - height);
                            
                            if (heightDifference > 1) continue;

                            var neighbour = new Tile(pos, height, tile.cost + heightDifference + 1);
                            
                            if (!world.IsWalkable(pos)) continue;

                            if (neighbour.cost <= TurnManager.instance.ActionPoints.ActionPoints)
                            {
                                tiles[pos] = neighbour;
                                queue.Enqueue(neighbour.gridPos);
                            }
                        }
                    }
                }
            }

            foreach (var tile in tiles.Values)
            {
                s.HighlightTargetPosition(tile.gridPos);
            }
        }
    }
}