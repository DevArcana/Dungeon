using System;
using System.Collections.Generic;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI
{
    public class CoverMap
    {
        private readonly Dictionary<GridPos, CoverType> _coverMap;
        private readonly List<GridPos> _softCovers;
        private readonly List<GridPos> _mediumCovers;
        private readonly List<GridPos> _hardCovers;
        
        public CoverMap(GridLivingEntity entity, List<GridPos> influencedPositions, List<GridLivingEntity> players)
        {
            _coverMap = new Dictionary<GridPos, CoverType>();
            _softCovers = new List<GridPos>();
            _mediumCovers = new List<GridPos>();
            _hardCovers = new List<GridPos>();
            
            const int distance = 5;
            foreach (var pos in influencedPositions)
            {
                var consideredPlayers = new List<GridLivingEntity>();
                foreach (var player in players)
                {
                    if (entity.GridPos.TwoDimDistance(player.GridPos) <= distance) consideredPlayers.Add(player); 
                }
                
                // soft covers
                foreach (var player in consideredPlayers)
                {
                    var isVisible = IsEntityVisible(player.GridPos, entity);
                    _coverMap.Add(pos, isVisible ? CoverType.NoCover : CoverType.SoftCover);
                    if (!isVisible)
                    {
                        _softCovers.Add(pos);
                    }
                }
            }
            
            foreach (var softCoverPos in _softCovers)
            {
                var consideredPlayers = new List<GridLivingEntity>();
                foreach (var player in players)
                {
                    if (entity.GridPos.TwoDimDistance(player.GridPos) <= distance) consideredPlayers.Add(player); 
                }

                var positionsToCheck = new HashSet<GridPos>();
                
            }
        }

        private bool IsEntityVisible(GridPos observerGridPos, GridLivingEntity entity)
        {
            var world = World.World.instance;
            // TODO: check if height in world is the same as in game
            var observerPosition = new Vector3(observerGridPos.x, world.GetHeightAt(observerGridPos) + 0.5f, observerGridPos.y);
            var entityPosition = entity.transform.position + new Vector3(0, 0.5f, 0);

            if (!Physics.Raycast(observerPosition, entityPosition - observerPosition, out var hit))
            {
                return false;
            }
            var hitEntity = hit.transform.GetComponent<GridLivingEntity>();
            return hitEntity == entity;
        }
    }
}