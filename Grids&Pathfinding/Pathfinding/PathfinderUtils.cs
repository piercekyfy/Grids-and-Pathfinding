using System.Collections.Generic;
using UnityEngine;
using PWH.Grids;

namespace PWH.Grids.Pathfinding
{
    public static class PathfinderUtils
    {
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

        public static Grid<IPathfindingNode> ConvertGridToGraph<T>(SquareGrid<T> grid) where T : IPathfindingNode
        {
            Grid<IPathfindingNode> graph = new SquareGrid<IPathfindingNode>(grid.Gridaxis, grid.WorldSpaceOffset, grid.Width, grid.Height,
                (_,__,___) =>
                {
                    return null;
                }, grid.WorldSpaceCellSize, grid.ConnectDiagonally);


            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    graph.Map[x, y] = grid.GetValue(x, y);
                }
            }

            return graph;
        }

        public static Grid<IPathfindingNode> ConvertGridToGraph<T>(HexagonGrid<T> grid) where T : IPathfindingNode
        {
            Grid<IPathfindingNode> graph = new HexagonGrid<IPathfindingNode>(grid.Gridaxis, grid.WorldSpaceOffset, grid.Width, grid.Height,
                (_, __, ___) =>
                {
                    return null;
                }, grid.WorldSpaceCellSize);


            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    graph.Map[x, y] = grid.GetValue(x, y);
                }
            }

            return graph;
        }
    }
}


