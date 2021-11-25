using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public class OffensiveBucket : IBucket
    {
        public float EvaluateBucketUtility(EnemyEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public (ActionType, GridPos?) EvaluateBucketActions(EnemyEntity entity)
        {
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);
            var entities = TurnManager.instance.PeekQueue();
            var players = entities.OfType<PlayerEntity>().Select(currentEntity => entity).Cast<GridLivingEntity>().ToList();
            var coverMap = new CoverMap(entity, InfluenceMap.instance.GetEntityInfluencedPos(entity), players).GetCoverMap();
            
            var utilities = new List<(ActionType, float)>
            {
                (ActionType.ChargePlayer, UtilityFunctions.ChargePlayerUtility(entity, targetEntity, out var chargeTarget)),
                (ActionType.Fireball, UtilityFunctions.FireballUtility(entity, out var fireballTarget)),
                (ActionType.TacticalMovement, UtilityFunctions.TacticalMovementUtility(entity, coverMap, out var tacticalMoveTarget)),
                (ActionType.Pass, UtilityFunctions.PassTurnUtility())
            }.Where(x => x.Item2 > 0.04f).OrderByDescending(x => x.Item2).ToList();

            var result = Helpers.WeightedRandom(utilities);
            
            return result switch
            {
                ActionType.ChargePlayer => (result, chargeTarget),
                ActionType.Fireball => (result, fireballTarget),
                ActionType.TacticalMovement => (result, tacticalMoveTarget),
                _ => (result, null)
            };
        }
    }
}