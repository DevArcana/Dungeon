using System.Collections.Generic;
using System.Linq;
using EntityLogic.Abilities;
using EntityLogic.Abilities.ReadyAbilities;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI
{
    public static class UtilityFunctions
    {
        public static float ChargePlayerUtility(EnemyEntity entity, GridLivingEntity targetEntity, out GridPos target)
        {
            target = targetEntity.GridPos;
            if (entity.currentTurnActions.Contains(ActionType.TacticalMovement)) return 0f;
            
            var availableActionPoints = TurnManager.instance.ActionPoints.ActionPoints;
            var maxChargeDistance = 10f + entity.attributes.WeaponRange;
            var influenceMap = InfluenceMap.instance;
            var pathfinding = new Pathfinding();
            var (path, cost, fullCost) = pathfinding.FindPartialPath(entity.GridPos, targetEntity.GridPos,
                availableActionPoints, (int)maxChargeDistance);

            if (path is null || !path.Any()) return 0f;
            target = path[path.Count - 1];
            var costFactor = cost == fullCost
                ? 1 - Mathf.Pow(fullCost / maxChargeDistance, 3)
                : (1 - Mathf.Pow(fullCost / maxChargeDistance, 2)) / 3;

            var recalculatedInfluenceOnPos = influenceMap.GetInfluenceOnPos(target).overallInfluence
                - influenceMap.GetEntityInfluenceOnPos(entity, target) + (cost == fullCost ? 0.8f : 1f);
            
            var influenceFactor = 1 / (1f + Mathf.Pow(2.718f, -(recalculatedInfluenceOnPos * 6) + 0.5f));
            
            var health = entity.health;
            var healthPercentage = health.Health / health.MaximumHealth;
            var healthFactor = 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 5.5f));

            return healthFactor * ((costFactor + influenceFactor) / 2f);
        }
        
        public static float HealSelfUtility(EnemyEntity entity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            if (!abilityProcessor.SelectAbility<HealSelfAbility>()) return 0f;
            if (!abilityProcessor.CanExecute(entity.GridPos))
            {
                abilityProcessor.DeselectAbility();
                return 0f;
            }
            var influenceMap = InfluenceMap.instance;
            var health = entity.health;

            var healthPercentage = health.Health / health.MaximumHealth;
            // logistic function
            var healthFactor = 1 - 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 5.5f));

            var playerInfluence = influenceMap.GetInfluenceOnPos(entity.GridPos).playersInfluence;
            // logistic function
            var threat = 1 / (1 + Mathf.Pow(2.718f * 1.2f, -(playerInfluence * 12) + 7f));
            
            abilityProcessor.DeselectAbility();
            return healthFactor * (1 - threat);
        }

        public static float HealAllyUtility(EnemyEntity entity, out GridPos target)
        {
            target = entity.GridPos;
            var abilityProcessor = AbilityProcessor.instance;
            if (!abilityProcessor.SelectAbility<HealAllyAbility>()) return 0f;
            
            var ability = abilityProcessor.SelectedAbility;
            var influenceMap = InfluenceMap.instance;
            var map = World.World.instance;

            var targets = abilityProcessor.SelectedAbility.GetValidTargetPositions().ToList();
            if (ability.GetMinimumPossibleCost() > TurnManager.instance.ActionPoints.ActionPoints || !targets.Any())
            {
                abilityProcessor.DeselectAbility();
                return 0f;
            }

            var bestScore = 0f;
            
            foreach (var currentTarget in targets)
            {
                if (!abilityProcessor.CanExecute(currentTarget)) continue;
                var currentEntity = map.GetOccupant(currentTarget);
                var health = currentEntity.health;
                var healthPercentage = health.Health / health.MaximumHealth;
                var healthFactor = 1 - 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 5.5f));
                var playerInfluence = influenceMap.GetInfluenceOnPos(currentTarget).playersInfluence;
                var threat = 1 / (1 + Mathf.Pow(2.718f * 1.2f, -(playerInfluence * 12) + 7f));
                var score = healthFactor * threat;
                if (score > bestScore)
                {
                    bestScore = score;
                    target = currentTarget;
                }
            }

            abilityProcessor.DeselectAbility();
            return bestScore;
        }

        public static float RetreatUtility(EnemyEntity entity, Dictionary<GridPos, CoverType> coverMap, out GridPos target)
        {
            // var abilityProcessor = AbilityProcessor.instance;
            var influenceMap = InfluenceMap.instance;
            var health = entity.health;
            target = entity.GridPos;

            var healthPercentage = health.Health / health.MaximumHealth;
            // logistic function
            var healthFactor = 1 - 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 4f));

            var influence = influenceMap.GetInfluenceOnPos(entity.GridPos);
            // exponential function
            var threat = Mathf.Min(Mathf.Pow(influence.playersInfluence / 0.8f , 4), 1);
            // if threat is above average
            if (!(threat > 0.5f)) return 0f;
            
            var bestScore = 0f;
            var positions = new Dictionary<GridPos, float>();
            foreach (var position in influenceMap.GetEntityInfluencedPos(entity))
            {
                var occupant = World.World.instance.GetOccupant(position);
                if (!(occupant is null) && occupant != entity) continue;
                var coverFactor = coverMap[position] == CoverType.MediumCover ? 0.75f :
                    coverMap[position] == CoverType.SoftCover ? 0.4f :
                    coverMap[position] == CoverType.NoCover ? 0f : 1f;
                var currentInfluence = influenceMap.GetInfluenceOnPos(position);
                var threatFactor = 1 - Mathf.Min(Mathf.Pow(currentInfluence.playersInfluence / 0.8f , 4), 1);
                var alliance = currentInfluence.agentsInfluence -
                               influenceMap.GetEntityInfluenceOnPos(entity, position);
                var allianceFactor = Mathf.Min(Mathf.Pow(alliance / 0.8f , 4), 1);
                var score = (coverFactor + threatFactor + allianceFactor) / 3f;
                if (score > bestScore)
                {
                    bestScore = score;
                    target = position;
                }
                positions[position] = (coverFactor + threatFactor + allianceFactor) / 3f;
            }

            if (bestScore > positions[entity.GridPos] * 1.2)
            {
                return healthFactor * threat;
            }
            return 0f;
        }

        public static float FireballUtility(EnemyEntity entity, out GridPos target)
        {
            target = entity.GridPos;
            var abilityProcessor = AbilityProcessor.instance;
            if (!abilityProcessor.SelectAbility<FireballAbility>()) return 0f;

            var ability = abilityProcessor.SelectedAbility;
            var influenceMap = InfluenceMap.instance;
            var map = World.World.instance;
            var targets = ability.GetValidTargetPositions().ToList();

            if (ability.GetMinimumPossibleCost() > TurnManager.instance.ActionPoints.ActionPoints || !targets.Any())
            {
                abilityProcessor.DeselectAbility();
                return 0f;
            }

            var bestScore = 0f;
            
            var influenceFactor = 1 / (1f + Mathf.Pow(2.718f, -(influenceMap.GetInfluenceOnPos(target).overallInfluence * 6) + 0.5f));

            var health = entity.health;
            var healthPercentage = health.Health / health.MaximumHealth;
            var healthFactor = 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 5.5f));
                
            var skillDamage = ((FireballAbility) ability).CalculateDamage();
            
            foreach (var currentTarget in targets)
            {
                var playerHealth = map.GetOccupant(currentTarget).health.Health;
                var playerHealthFactor = Mathf.Min(Mathf.Pow(0.5f, playerHealth / (skillDamage / 2f)) + 0.7f, 1f);

                var score = (healthFactor * influenceFactor + playerHealthFactor) / 2f;
                if (score > bestScore)
                {
                    bestScore = score;
                    target = currentTarget;
                }
            }
            
            abilityProcessor.DeselectAbility();
            return bestScore;
        }

        public static float TacticalMovementUtility(EnemyEntity entity, GridLivingEntity targetEntity, Dictionary<GridPos, CoverType> coverMap, out GridPos target)
        {
            target = entity.GridPos;
            
            var influenceMap = InfluenceMap.instance;
            var availableActionPoints = TurnManager.instance.ActionPoints.ActionPoints;
            var pathfinding = new Pathfinding();
            var health = entity.health;
            var healthPercentage = health.Health / health.MaximumHealth;
            var healthFactor = 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 5.5f));

            if (healthFactor < 0.05f) return 0f;
            // var positions = new Dictionary<GridPos, float>();
            var teamworkFactor = entity.teamwork - 0.5f;

            var bestScore = 0f;
            var (path, cost, fullCost) = pathfinding.FindPartialPath(entity.GridPos, targetEntity.GridPos, availableActionPoints, includeEnemyCost: false);
            if (path is null) return 0f;
            var closestPoint = path[path.Count - 1];
            var positionsToCheck = new HashSet<GridPos>(closestPoint.Circle(3));
            positionsToCheck.IntersectWith(influenceMap.GetEntityInfluencedPos(entity));

            foreach (var position in positionsToCheck)
            {
                var occupant = World.World.instance.GetOccupant(position);
                if (!(occupant is null) && occupant != entity) continue;
                var coverFactor = coverMap[position] == CoverType.HardCover ? 1f :
                    coverMap[position] == CoverType.MediumCover ? 0.75f :
                    coverMap[position] == CoverType.SoftCover ? 0.4f : 0f;
                var currentInfluence = influenceMap.GetInfluenceOnPos(position);
                var threatFactor = 1 - Mathf.Min(Mathf.Pow(currentInfluence.playersInfluence / 0.8f , 4), 1);
                var alliance = currentInfluence.agentsInfluence -
                               influenceMap.GetEntityInfluenceOnPos(entity, position);
                var allianceFactor = Mathf.Max(-Mathf.Pow((alliance - 0.4f) * 2.5f, 2) + 1, 0);
                var (_, distance) = pathfinding.FindPath(position, targetEntity.GridPos);
                var distanceFactor = ((fullCost - cost) / (float)distance) * ((fullCost - cost) / (float)distance);
                var score = threatFactor * ((0.5f * coverFactor + (1 + teamworkFactor) * allianceFactor + 1.5f * distanceFactor)  / (3f + teamworkFactor));
                if (score > bestScore)
                {
                    bestScore = score;
                    target = position;
                }
            }

            if (target == entity.GridPos) return 0f;
            return healthFactor * bestScore;
        }
        
        public static float PassTurnUtility() => 0.05f;
    }
}
