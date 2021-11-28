using System.Collections.Generic;
using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public interface IBucket
    {
        public float Score { get; }
        (ActionType, GridPos?) EvaluateBucketActions(EnemyEntity entity);
    }
}
