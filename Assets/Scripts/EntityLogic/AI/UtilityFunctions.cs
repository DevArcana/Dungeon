using System;
using System.Collections.Generic;
using EntityLogic.Abilities;
using EntityLogic.Abilities.ReadyAbilities;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI
{
    public static class UtilityFunctions
    {
        public static float MeleeAttackUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            var pathfinding = new Pathfinding();
            if (abilityProcessor.CanExecute(targetEntity.GridPos)
                && entity.GridPos.OneDimDistance(targetEntity.GridPos) == 1
                && pathfinding.FindPath(entity.GridPos, targetEntity.GridPos, 1).Item2 == 1)
            {
                return 1f;
            }
            return 0f;
        }

        public static float RushPlayerUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            
            if (!abilityProcessor.CanExecute(targetEntity.GridPos) ||
                entity.GridPos.OneDimDistance(targetEntity.GridPos) == 1) return 0f;
            var availableActionPoints = TurnManager.instance.ActionPoints.ActionPoints;
            var maxCost = availableActionPoints + 2;
            var cost = abilityProcessor.SelectedAbility.GetEffectiveCost(targetEntity.GridPos);
            if (cost <= 0 || cost > maxCost) return 0f;

            return Mathf.Pow(cost / (float) maxCost, 0.333f);
        }
        
        public static float HealSelfUtility(EnemyEntity entity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            if (!abilityProcessor.SelectAbility(typeof(HealSelfAbility)) || !abilityProcessor.CanExecute(entity.GridPos)) return 0f;
            var influenceMap = InfluenceMap.instance;
            var health = entity.GetComponent<DamageableEntity>().damageable;

            var healthPercentage = health.Health / (float) health.MaxHealth;
            // logistic function
            var healthFactor = 1 - 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 5.5f));

            var playerInfluence = influenceMap.GetInfluenceOnPos(entity.GridPos).playersInfluence;
            // logistic function
            var threat = 1 / (1 + Mathf.Pow(2.718f * 1.2f, -(playerInfluence * 12) + 7f));
            
            abilityProcessor.DeselectAbility();
            return healthFactor * (1 - threat);
        }

        public static float RetreatUtility(EnemyEntity entity, Dictionary<GridPos, CoverType> coverMap, out GridPos target)
        {
            // var abilityProcessor = AbilityProcessor.instance;
            var influenceMap = InfluenceMap.instance;
            var health = entity.GetComponent<DamageableEntity>().damageable;
            target = entity.GridPos;

            var healthPercentage = health.Health / (float) health.MaxHealth;
            // logistic function
            var healthFactor = 1 - 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 4f));

            var influence = influenceMap.GetInfluenceOnPos(entity.GridPos);
            // exponential function
            var threat = Mathf.Min(Mathf.Pow(influence.playersInfluence / 0.8f , 4), 1);
            // if threat is above average
            if (!(threat > 0.5f)) return 0f;
            
            var bestScore = 0f;
            var positions = new Dictionary<GridPos, float>();
            var aa = influenceMap.GetEntityInfluencedPos(entity);
            if (!aa.Contains(entity.GridPos))
            {
                var a = 1 + 1;
            }
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
        
        public static float PassTurnUtility() => 0.05f;
    }
}