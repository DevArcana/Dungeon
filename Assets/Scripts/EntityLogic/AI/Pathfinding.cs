﻿using System;
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

        public (List<GridPos>, int) FindPath(GridPos start, GridPos end, int maxCost = ActionPointsProcessor.MaxActionPoints * 3)
        {
            var map = World.World.instance;
            var startNode = new PathNode(start.x, start.y, map.GetHeightAt(start), !map.IsOccupied(start));
            var endNode = new PathNode(end.x, end.y, map.GetHeightAt(end), !map.IsOccupied(end));

            var openList = new Dictionary<PathNode, PathNode> { {startNode, startNode} };
            var closedList = new HashSet<PathNode>();

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Any())
            {
                var currentNode = GetLowestFCostNode(openList.Keys);
                if (currentNode == endNode)
                {
                    var path = GetPath(currentNode);
                    return (path.Select(node => GridPos.At(node.x, node.y)).ToList(), Mathf.FloorToInt(currentNode.gCost));
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (var neighbour in GetNeighbourNodes(currentNode))
                {
                    if (closedList.Contains(neighbour)) continue;
                    var neighbourNode = openList.ContainsKey(neighbour) ? openList[neighbour] : neighbour;
                    if (!neighbourNode.isWalkable && neighbourNode != endNode)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }
                    var tempGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tempGCost >= neighbourNode.gCost
                        || Mathf.Abs(currentNode.height - neighbourNode.height) > 1
                        || Mathf.Floor(tempGCost) > maxCost) continue;
                    neighbourNode.previousNode = currentNode;
                    neighbourNode.gCost = tempGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
                    
                    openList[neighbourNode] = neighbourNode;
                }
            }

            return (null, 0);
        }

        public HashSet<PathNode> GetShortestPathTree(GridPos start, int maxCost = ActionPointsProcessor.MaxActionPoints)
        {
            var map = World.World.instance;
            var startNode = new PathNode(start.x, start.y, map.GetHeightAt(start), !map.IsOccupied(start));
            
            var openList = new Dictionary<PathNode, PathNode> { {startNode, startNode} };
            var resultList = new HashSet<PathNode> { startNode };
            var closedList = new HashSet<PathNode>();
            
            startNode.gCost = startNode.fCost = 0;
            
            while (openList.Any())
            {
                var currentNode = GetLowestFCostNode(openList.Keys);
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                
                foreach (var neighbour in GetNeighbourNodes(currentNode))
                {
                    if (closedList.Contains(neighbour)) continue;
                    var neighbourNode = openList.ContainsKey(neighbour) ? openList[neighbour] : neighbour;
                    var isOccupied = !(map.GetOccupant(GridPos.At(neighbourNode.x, neighbourNode.y)) is null);
                    if (!neighbourNode.isWalkable && !isOccupied)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }
                    var tempGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    // if a position is occupied and is on a different height than the neighbour, it might be problematic since
                    // here we are assuming that we sort of treat occupied positions as walkable, so when this function will be
                    // used in highlighting available moves and melee attacks, the shortest path might be the one that requires
                    // changing the height right before the attack action which is not possible. This of course does not interfere
                    // with the attack itself, but it might cause some visual bugs in highlighting. I leave it here to investigate
                    // if it will be actually observed and problematic, because I do not need to adjust for such a problem for now.
                    if (tempGCost >= neighbourNode.gCost
                        || Mathf.Abs(currentNode.height - neighbourNode.height) > 1
                        || Mathf.Floor(tempGCost) > maxCost) continue;
                    neighbourNode.previousNode = currentNode;
                    neighbourNode.gCost = neighbourNode.fCost = tempGCost;

                    if (!isOccupied) openList[neighbourNode] = neighbourNode;
                    resultList.Add(neighbourNode);
                }
                
            }

            foreach (var pathNode in resultList)
            {
                pathNode.gCost = Mathf.FloorToInt(pathNode.gCost);
            }

            return resultList;
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

        private static PathNode GetLowestFCostNode(IEnumerable<PathNode> pathNodes)
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
                    var node = new PathNode(x, y, map.GetHeightAt(pos), !map.IsOccupied(pos));
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