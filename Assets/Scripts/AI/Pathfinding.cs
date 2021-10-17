using System;
using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace AI
{
    public class Pathfinding
    {
        private const int RegularMoveCost = 1;
        private const float DiagonalPenalizedCost = 1.1f;
        private const int ClimbCost = 1;
        
        private readonly PathNode[,] _grid;

        private List<PathNode> _openList;
        private List<PathNode> _closedList;
        
        public Pathfinding(int distance, GridPos start)
        {
            var map = World.World.instance;
            _grid = map.GetAreaAround(distance, start);
        }

        public IEnumerable<GridPos> FindPath(GridPos end)
        {
            var middle = _grid.GetLength(0) / 2;
            var startNode = _grid[middle, middle];

            var xDistance = startNode.x - end.x;
            var yDistance = startNode.y - end.y;
            var endX = middle - xDistance;
            var endY = middle - yDistance;

            var endNode = _grid[endX, endY];

            _openList = new List<PathNode> { startNode };
            _closedList = new List<PathNode>();

            for (var x = 0; x < _grid.GetLength(0); x++)
            {
                for (var y = 0; y < _grid.GetLength(1); y++)
                {
                    var pathNode = _grid[x, y];
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.previousNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (_openList.Any())
            {
                var currentNode = GetLowestFCostNode(_openList);
                if (currentNode == endNode)
                {
                    var path = GetPath(endNode);
                    for (var i = 0; i < _grid.GetLength(0); i++)
                    {
                        for (var j = 0; j < _grid.GetLength(1); j++)
                        {
                            var node = _grid[i, j];

                            if (startNode == node)
                            {
                                Debug.DrawRay(new Vector3(node.x, node.height, node.y), Vector3.up, Color.blue, 10);
                            }
                            else if (endNode == node)
                            {
                                Debug.DrawRay(new Vector3(node.x, node.height, node.y), Vector3.up, Color.magenta, 10);
                            }
                            else
                            {
                                var isPath = path.Contains(node);
                                Debug.DrawRay(new Vector3(node.x, node.height, node.y), Vector3.up, isPath ? Color.green : Color.red, 10);
                            }
                        }
                    }
                    return path.Select(node => GridPos.At(node.x, node.y));
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
                    if (tempGCost >= neighbourNode.gCost || Mathf.Abs(currentNode.height - neighbourNode.height) > 1) continue;
                    neighbourNode.previousNode = currentNode;
                    neighbourNode.gCost = tempGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!_openList.Contains(neighbourNode))
                    {
                        _openList.Add(neighbourNode);
                    }
                }
            }

            return null;
        }

        private float CalculateDistanceCost(PathNode a, PathNode b)
        {
            var xDistance = Mathf.Abs(a.x - b.x);
            var yDistance = Mathf.Abs(a.y - b.y);
            var diagonalMoves = Mathf.Min(xDistance, yDistance);
            var straightMoves = Mathf.Abs(xDistance - yDistance);
            var heightDifference = Mathf.Abs(b.height - a.height);

            return straightMoves * RegularMoveCost + diagonalMoves * DiagonalPenalizedCost + heightDifference * ClimbCost;
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodes)
        {
            var lowestFCostNode = pathNodes[0];
            for (var i = 1; i < pathNodes.Count; i++)
            {
                var currentNode = pathNodes[i];
                if (currentNode.fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = currentNode;
                }
            }

            return lowestFCostNode;
        }
        
        private List<PathNode> GetPath(PathNode endNode)
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

        private List<PathNode> GetNeighbourNodes(PathNode currentNode)
        {
            var neighbourNodes = new List<PathNode>();
            
            // W
            if (currentNode.relativeX - 1 >= 0)
            {
                neighbourNodes.Add(_grid[currentNode.relativeX - 1, currentNode.relativeY]);
                // NW
                if (currentNode.relativeY - 1 >= 0)
                {
                    neighbourNodes.Add(_grid[currentNode.relativeX - 1, currentNode.relativeY - 1]);
                }
            }
            // E
            if (currentNode.relativeX + 1 < _grid.GetLength(0))
            {
                neighbourNodes.Add(_grid[currentNode.relativeX + 1, currentNode.relativeY]);
                // SE
                if (currentNode.relativeY + 1 < _grid.GetLength(1))
                {
                    neighbourNodes.Add(_grid[currentNode.relativeX + 1, currentNode.relativeY + 1]);
                }
            }
            // N
            if (currentNode.relativeY - 1 >= 0)
            {
                neighbourNodes.Add(_grid[currentNode.relativeX, currentNode.relativeY - 1]);
                // NE
                if (currentNode.relativeX + 1 < _grid.GetLength(0))
                {
                    neighbourNodes.Add(_grid[currentNode.relativeX + 1, currentNode.relativeY - 1]);
                }
            }
            // S
            if (currentNode.relativeY + 1 < _grid.GetLength(1))
            {
                neighbourNodes.Add(_grid[currentNode.relativeX, currentNode.relativeY + 1]);
                // SW
                if (currentNode.relativeX - 1 >= 0)
                {
                    neighbourNodes.Add(_grid[currentNode.relativeX - 1, currentNode.relativeY + 1]);
                }
            }
            
            return neighbourNodes;
        }

        public static GridPos FindClosestPlayer(GridPos startPos)
        {
            var map = World.World.instance;
            var entities = map.GetAllMapEntities();
            var lowestDistance = int.MaxValue;
            GridPos target = new GridPos(0,0);
            foreach (var gridTile in entities)
            {
                if (gridTile.Value.OfType<PlayerEntity>().Any())
                {
                    var distance = Mathf.Abs(startPos.x - gridTile.Key.x) + Mathf.Abs(startPos.y - gridTile.Key.y);
                    if (distance >= lowestDistance) continue;
                    lowestDistance = distance;
                    target = gridTile.Key;
                }
            }

            return target;
        }
    }
}