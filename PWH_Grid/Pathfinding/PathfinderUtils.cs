using System.Collections.Generic;
using UnityEngine;
using PWH.Grid;

namespace PWH.Grid.Pathfinding
{
    public static class PathfinderUtils
    {
        public static readonly Vector2[] allDirections =
        {
            new Vector2(0f,1f),
            new Vector2(1f,1f),
            new Vector2(1f,0f),
            new Vector2(1f,-1f),
            new Vector2(0f,-1f),
            new Vector2(-1f,-1f),
            new Vector2(-1f,-0f),
            new Vector2(-1f,1f),
        };

        public static readonly Vector2[] uniformDirections =
        {
            new Vector2(0f,1f),
            new Vector2(1f,0f),
            new Vector2(0f,-1f),
            new Vector2(-1f,-0f),
        };

        public static float GetNodeDistance(IPathfindingNode source, IPathfindingNode target)
        {
            int dx = Mathf.Abs(source.xIndex - target.xIndex);
            int dy = Mathf.Abs(source.yIndex - target.yIndex);

            int min = Mathf.Min(dx, dy);
            int max = Mathf.Max(dx, dy);

            int diagonalSteps = min;
            int straightSteps = max - min;

            return (1.4f * diagonalSteps + straightSteps);
        }

        public static List<IPathfindingNode> ToIPathfindingNodes<T>(List<T> nodes)
        {
            List<IPathfindingNode> newList = new List<IPathfindingNode>();
            foreach (T node in nodes)
            {
                if (node is IPathfindingNode)
                {
                    newList.Add((IPathfindingNode)node);
                }
            }

            return newList;
        }

        public static List<IPathfindingNode> FilterTraversable(List<IPathfindingNode> nodes)
        {
            List<IPathfindingNode> newList = new List<IPathfindingNode>();
            foreach (IPathfindingNode node in nodes)
            {
                if (node.traversable)
                {
                    newList.Add(node);
                }
            }
            return newList;
        }

        public static GenericGrid<IPathfindingNode> ConvertGridToGraph<T>(GenericGrid<T> grid) where T : System.IEquatable<T>
        {
            GenericGrid<IPathfindingNode> graph = new GenericGrid<IPathfindingNode>(Vector2.zero,grid.width, grid.height,
                (GenericGrid<IPathfindingNode> source, int x, int y) =>
                {
                    return null;
                });

            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    if (grid.GetValue(x, y,out bool foundValue) is IPathfindingNode)
                    {
                        graph.map[x, y] = (IPathfindingNode)grid.GetValue(x, y,out bool _foundValue);
                    }
                }
            }

            return graph;
        }
    }
}


