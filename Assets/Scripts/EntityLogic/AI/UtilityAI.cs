using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;
using Random = UnityEngine.Random;

namespace EntityLogic.AI
{
    public class UtilityAI
    {
        private ActionType ChooseNextAction(EnemyEntity entity)
        {
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);
            var utilities = new List<(ActionType, float)>
            {
                (ActionType.Attack, MeleeAttackUtility(entity, targetEntity)),
                (ActionType.Run, RushPlayerUtility(entity, targetEntity)),
                (ActionType.Pass, PassTurnUtility(entity))
            }.Where(x => x.Item2 != 0f).OrderByDescending(x => x.Item2).ToList();

            Debug.Log("Scores:");
            foreach (var (action, score) in utilities)
            {
                Debug.Log($"{action} - {score}");
            }

            var (_, bestUtilityScore) = utilities.First();
            var bestUtilities = utilities
                .Where(x => x.Item2 >= bestUtilityScore * 0.9f)
                .ToList();
            var totalScore = 0f;

            foreach (var (_, score) in bestUtilities)
            {
                totalScore += score;
            }

            var choice = Random.Range(0f, totalScore);
            var accumulator = 0f;
            
            foreach (var (action, score) in bestUtilities)
            {
                accumulator += score;
                if (choice < accumulator) return action;
            }

            return ActionType.Pass;
        }

        public void PerformNextAction(EnemyEntity entity)
        {
            TransactionBase transaction;
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);

            switch (ChooseNextAction(entity))
            {
                case ActionType.Attack:
                    transaction = new AttackTransaction(entity, targetEntity, 5);
                    break;
                case ActionType.Run:
                    var pathfinding = new Pathfinding();
                    var (path, _) = pathfinding.FindPath(entity.GridPos, targetEntity.GridPos);
                    path.RemoveAt(path.Count - 1);
                    transaction = new MoveTransaction(entity, GridPos.At(path[0].x, path[0].y));
                    break;
                case ActionType.Pass:
                    transaction = new PassTurnTransaction(entity);
                    break;
                default:
                    Debug.Log("No action found");
                    transaction = new PassTurnTransaction(entity);
                    break;
            }
            
            TurnManager.instance.Transactions.EnqueueTransaction(transaction);
        }

        private float MeleeAttackUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        {
            var testTransaction = new AttackTransaction(entity, targetEntity, 0);
            //TODO: fix if (!TurnManager.instance.CanProcessTransaction(testTransaction)) return 0f;
            return 1f;
        }

        private float RushPlayerUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        {
            var pathfinding = new Pathfinding();
            var (_, cost) = pathfinding.FindPath(entity.GridPos, targetEntity.GridPos);
            const int maxCost = ActionPointsProcessor.MaxActionPoints;
            var map = World.World.instance;
            var heightDifference = Mathf.Abs(map.GetHeightAt(entity.GridPos) - map.GetHeightAt(targetEntity.GridPos));
            if (cost > maxCost || cost == 1 || heightDifference == 1 && cost == 2) return 0f;
            return Mathf.Pow(cost / (float) maxCost, 0.333f);
        }

        private float PassTurnUtility(EnemyEntity entity)
        {
            return 0.05f;
        }
    }
}