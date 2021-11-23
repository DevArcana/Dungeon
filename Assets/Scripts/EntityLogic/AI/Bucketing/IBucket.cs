using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public interface IBucket
    {
        float EvaluateBucketUtility(EnemyEntity entity);
        (ActionType, GridPos?) EvaluateBucketActions(EnemyEntity entity);
    }
}