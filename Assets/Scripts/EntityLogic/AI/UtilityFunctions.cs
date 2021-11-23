using System;
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
        // public static float MeleeAttackUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        // {
        //     var abilityProcessor = AbilityProcessor.instance;
        //     var pathfinding = new Pathfinding();
        //     if (abilityProcessor.CanExecute(targetEntity.GridPos)
        //         && entity.GridPos.OneDimDistance(targetEntity.GridPos) == 1
        //         && pathfinding.FindPath(entity.GridPos, targetEntity.GridPos, 1).Item2 == 1)
        //     {
        //         return 1f;
        //     }
        //     return 0f;
        // }

        public static float ChargePlayerUtility(EnemyEntity entity, GridLivingEntity targetEntity, out GridPos target)
        {
            target = targetEntity.GridPos;
            var availableActionPoints = TurnManager.instance.ActionPoints.ActionPoints;
            var maxChargeDistance = 10f + (entity.equipment.weapon ? entity.equipment.weapon.range : 0);
            var pathfinding = new Pathfinding();
            var (path, cost, fullCost) = pathfinding.FindPartialPath(entity.GridPos, targetEntity.GridPos,
                availableActionPoints, (int)maxChargeDistance);

            if (path is null || !path.Any()) return 0f;
            target = path[path.Count - 1];
            if (cost == fullCost)
            {
                return 1 - Mathf.Pow(fullCost / maxChargeDistance, 3);
            }
            return (1 - Mathf.Pow(fullCost / maxChargeDistance, 2)) / 2;
        }
        
        public static float HealSelfUtility(EnemyEntity entity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            if (!abilityProcessor.SelectAbility(typeof(HealSelfAbility))) return 0f;
            if (!abilityProcessor.CanExecute(entity.GridPos))
            {
                abilityProcessor.DeselectAbility();
                return 0f;
            }
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

        // public static float FireballUtility(EnemyEntity entity)
        // {
        //     var abilityProcessor = AbilityProcessor.instance;
        //     if (!abilityProcessor.SelectAbility(typeof(FireballAbility))) return 0f;
        //     
        //     var influenceMap = InfluenceMap.instance;
        //     var targets = abilityProcessor.SelectedAbility.GetValidTargetPositions();
        //
        //     if (!targets.Any()) return 0f;
        //
        //     if (!abilityProcessor.CanExecute(entity.GridPos))
        //     {
        //         abilityProcessor.DeselectAbility();
        //         return 0f;
        //     }
        // }
        
        public static float PassTurnUtility() => 0.05f;
    }
}