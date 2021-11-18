using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI
{
    public class CoverMap
    {
        private readonly Dictionary<GridPos, CoverType> _coverMap;

        public CoverMap(GridLivingEntity entity, List<GridPos> influencedPositions, List<GridLivingEntity> players)
        {
            _coverMap = new Dictionary<GridPos, CoverType>();
            var softCovers = new List<GridPos>();
            var mediumCovers = new List<GridPos>();
            
            const int distance = 7;
            foreach (var pos in influencedPositions)
            {
                var consideredPositions = players
                    .Select(player => player.GridPos)
                    .Where(playerPos => playerPos.TwoDimDistance(pos) <= distance)
                    .ToList();

                if (!consideredPositions.Any())
                {
                    _coverMap.Add(pos, CoverType.OutOfRange);
                    continue;
                }

                var isVisible = consideredPositions.Any(playerPos => IsEntityVisible(playerPos, pos));

                _coverMap.Add(pos, isVisible ? CoverType.NoCover : CoverType.SoftCover);
                if (!isVisible)
                {
                    softCovers.Add(pos);
                }

            }

            var influenceMap = InfluenceMap.instance;
            
            foreach (var softCoverPos in softCovers)
            {
                var consideredPlayers = new List<GridLivingEntity>();
                foreach (var player in players)
                {
                    if (entity.GridPos.TwoDimDistance(player.GridPos) <= distance) consideredPlayers.Add(player); 
                }

                var positionsToCheck = new HashSet<GridPos>();

                foreach (var player in consideredPlayers)
                {
                    positionsToCheck.UnionWith(influenceMap.GetInfluencedPosOfCost(player, 1));
                }
                
                var isVisible = positionsToCheck
                    .Where(pos => pos.TwoDimDistance(softCoverPos) <= 5)
                    .Any(pos => IsEntityVisible(pos, softCoverPos));

                if (isVisible) continue;
                _coverMap[softCoverPos] = CoverType.MediumCover;
                mediumCovers.Add(softCoverPos);
            }
            
            foreach (var mediumCoverPos in mediumCovers)
            {
                var consideredPlayers = new List<GridLivingEntity>();
                foreach (var player in players)
                {
                    if (entity.GridPos.TwoDimDistance(player.GridPos) <= distance) consideredPlayers.Add(player); 
                }

                var positionsToCheck = new HashSet<GridPos>();

                foreach (var player in consideredPlayers)
                {
                    positionsToCheck.UnionWith(influenceMap.GetInfluencedPosOfCost(player, 2));
                }

                var isVisible = positionsToCheck
                    .Where(pos => pos.TwoDimDistance(mediumCoverPos) <= 5)
                    .Any(pos => IsEntityVisible(pos, mediumCoverPos));

                if (isVisible) continue;
                _coverMap[mediumCoverPos] = CoverType.HardCover;
            }
        }

        private bool IsEntityVisible(GridPos observerGridPos, GridPos entityGridPos)
        {
            var world = World.World.instance;
            // TODO: check if height in world is the same as in game
            var observerPosition = new Vector3(observerGridPos.x, world.GetHeightAt(observerGridPos) + 0.5f, observerGridPos.y);
            var entityPosition = new Vector3(entityGridPos.x, world.GetHeightAt(entityGridPos) + 0.5f, entityGridPos.y);

            var distance = (entityPosition - observerPosition).magnitude;

            if (!Physics.Raycast(observerPosition, entityPosition - observerPosition, out var hit, distance,
                LayerMask.GetMask("Default", "Entities")))
            {
                return true;
            }

            return hit.transform.GetComponent<GridLivingEntity>()?.GridPos == entityGridPos;
        }

        public Dictionary<GridPos, CoverType> GetCoverMap()
        {
            return _coverMap;
        }
    }
}