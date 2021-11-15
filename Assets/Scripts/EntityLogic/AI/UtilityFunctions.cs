using System;
using EntityLogic.Abilities;
using EntityLogic.Abilities.ReadyAbilities;
using TurnSystem;
using UnityEngine;

namespace EntityLogic.AI
{
    public static class UtilityFunctions
    {
        public static float MeleeAttackUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            var availableActionPoints = TurnManager.instance.ActionPoints.ActionPoints;
            var pathfinding = new Pathfinding();
            if (entity.GridPos.OneDimDistance(targetEntity.GridPos) == 1
                && pathfinding.FindPath(entity.GridPos, targetEntity.GridPos, 1).Item2 == 1
                && availableActionPoints >= abilityProcessor.SelectedAbility.GetEffectiveCost(targetEntity.GridPos))
            {
                return 1f;
            }
            return 0f;
        }

        public static float RushPlayerUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            
            if (entity.GridPos.OneDimDistance(targetEntity.GridPos) == 1) return 0f;
            var availableActionPoints = TurnManager.instance.ActionPoints.ActionPoints;
            var maxCost = availableActionPoints + 2;
            var cost = abilityProcessor.SelectedAbility.GetEffectiveCost(targetEntity.GridPos);
            if (cost <= 0 || cost > maxCost) return 0f;

            return Mathf.Pow(cost / (float) maxCost, 0.333f);
        }
        
        public static float HealSelfUtility(EnemyEntity entity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            if (!abilityProcessor.SelectAbility(typeof(HealSelfAbility))) return 0f;
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

        public static float RetreatUtility(EnemyEntity entity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            var influenceMap = InfluenceMap.instance;
            var health = entity.GetComponent<DamageableEntity>().damageable;

            var healthPercentage = health.Health / (float) health.MaxHealth;
            // logistic function
            var healthFactor = 1 - 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 4f));

            var influence = influenceMap.GetInfluenceOnPos(entity.GridPos);
            // exponential function
            var threat = Mathf.Min(-(100 - Mathf.Pow(influence.playersInfluence * 125, 4)) / (100 ^ 4), 1);
            // if threat is above average
            if (threat > 0.5f)
            {
                
            }

            return 0f;
        }
        
        public static float PassTurnUtility() => 0.05f;
    }
}