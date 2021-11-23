using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EntityLogic.AI
{
    public class Helpers
    {
        public static ActionType WeightedRandom(List<(ActionType, float)> utilities)
        {
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
    }
}