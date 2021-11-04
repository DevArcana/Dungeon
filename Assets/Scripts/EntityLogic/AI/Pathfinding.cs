using System;
using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI
{
    public class Pathfinding
    {

        private const int RegularMoveCost = 1;
        private const float DiagonalPenalizedCost = 1.0001f;
        private const int ClimbCost = 1;
        
        private HashSet<PathNode> _openList;
        private HashSet<PathNode> _closedList;

        public (List<GridPos>, int) FindPath(GridPos start, GridPos end)
        {
            var map = World.World.instance;
            var startNode = new PathNode(start.x, start.y, map.GetHeightAt(start), map.IsWalkable(start));
            var endNode = new PathNode(end.x, end.y, map.GetHeightAt(end), map.IsWalkable(end));

            _openList = new HashSet<PathNode> { startNode };
            _closedList = new HashSet<PathNode>();

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (_openList.Any())
            {
                var currentNode = GetLowestFCostNode(_openList);
                if (currentNode == endNode)
                {
                    var path = GetPath(currentNode);
                    return (path.Select(node => GridPos.At(node.x, node.y)).ToList(), Mathf.FloorToInt(currentNode.gCost));
                }

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                foreach (var neighbourNode in GetNeighbourNodes(currentNode))
                {
                    if (_closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.isWalkable && neighbourNode != endNode)
                    {
                        _closedList.Add(neighbourNode);
                        continue;
                    }
                    var tempGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tempGCost >= neighbourNode.gCost
                        || Mathf.Abs(currentNode.height - neighbourNode.height) > 1
                        || Mathf.Floor(tempGCost) > ActionPointsProcessor.MaxActionPoints * 3) continue;
                    neighbourNode.previousNode = currentNode;
                    neighbourNode.gCost = tempGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
                    
                    _openList.Add(neighbourNode);
                }
            }

            return (null, 0);
        }

        private static float CalculateDistanceCost(PathNode a, PathNode b)
        {
            var xDistance = Mathf.Abs(a.x - b.x);
            var yDistance = Mathf.Abs(a.y - b.y);
            var diagonalMoves = Mathf.Min(xDistance, yDistance);
            var straightMoves = Mathf.Abs(xDistance - yDistance);
            var heightDifference = Mathf.Abs(b.height - a.height);

            return straightMoves * RegularMoveCost + diagonalMoves * DiagonalPenalizedCost + heightDifference * ClimbCost;
        }

        private static PathNode GetLowestFCostNode(HashSet<PathNode> pathNodes)
        {
            PathNode lowestFCostNode = null;
            foreach (var node in pathNodes)
            {
                if (lowestFCostNode is null)
                {
                    lowestFCostNode = node;
                    continue;
                }
                if (node.fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = node;
                }
            }
            return lowestFCostNode;
        }
        
        private static List<PathNode> GetPath(PathNode endNode)
        {
            var path = new List<PathNode> { endNode };
            var currentNode = endNode;
            while (currentNode.previousNode?.previousNode != null)
            {
                path.Add(currentNode.previousNode);
                currentNode = currentNode.previousNode;
            }

            path.Reverse();
            
            return path;
        }

        private static List<PathNode> GetNeighbourNodes(PathNode currentNode)
        {
            var neighbourNodes = new List<PathNode>();
            var map = World.World.instance;

            for (var x = currentNode.x - 1; x <= currentNode.x + 1; x++)
            {
                for (var y = currentNode.y - 1; y <= currentNode.y + 1; y++)
                {
                    if (x == currentNode.x && y == currentNode.y) continue;
                    var pos = GridPos.At(x, y);
                    var node = new PathNode(x, y, map.GetHeightAt(pos), map.IsWalkable(pos));
                    if (Math.Abs(node.gCost - default(float)) < 0.01f)
                    {
                        node.gCost = int.MaxValue;
                        node.CalculateFCost();
                        node.previousNode = null;
                    }
                    neighbourNodes.Add(node);
                }
            }
            return neighbourNodes;
        }

        public static GridLivingEntity FindClosestPlayer(GridPos startPos)
        {
            var entities = TurnManager.instance.PeekQueue().Where(e => e is PlayerEntity);
            var lowestDistance = int.MaxValue;
            
            GridLivingEntity target = null;
            
            foreach (var entity in entities)
            {
                var distance = Mathf.Abs(startPos.x - entity.GridPos.x) + Mathf.Abs(startPos.y - entity.GridPos.y);
                if (distance >= lowestDistance) continue;
                lowestDistance = distance;
                target = entity;
            }

            return target;
        }
    }
}