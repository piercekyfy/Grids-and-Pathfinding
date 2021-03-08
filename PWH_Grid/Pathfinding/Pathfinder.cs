using System.Collections.Generic;
using UnityEngine;
using PWH.Grid;
using PWH.Utils;

namespace PWH.Grid.Pathfinding
{
    public class Pathfinder : BasicSingleton<Pathfinder>
    {
        public Mode mode = Mode.AStar;

        [Header("Debug")]
        public Color DEBUG_Colour = Color.red;

        IPathfindingNode startNode;
        IPathfindingNode goalNode;

        GenericGrid<IPathfindingNode> graph;

        PriorityQueue<IPathfindingNode> frontierNodes;
        List<IPathfindingNode> exploredNodes;
        List<IPathfindingNode> pathNodes;

        bool isComplete = false;
        int iterations = 0;

        public enum Mode
        {
            BreadthFirstSearch,
            Dijkstra,
            GreedyBestFirstSearch,
            AStar
        }

        public List<IPathfindingNode> Init(GenericGrid<IPathfindingNode> graph,IPathfindingNode startNode,IPathfindingNode goalNode,bool DEBUG_showPath = false,float DEBUG_duration = 10f)
        {
            if (graph == null) { Debug.LogWarning("Pathfinder Error: Graph is null"); return null; }
            if (startNode == null) { Debug.LogWarning("Pathfinder Error: Start node is null"); return null; }
            if (goalNode == null) { Debug.LogWarning("Pathfinder Error: Goal node is null"); return null; }

            // Had to remove this line as I found checking if the startNode is traversable
            // got in the way with creating a cell inhabited system and I figure there is no reason to check it anyway
            // considering that if the start node is not traversable, you've probably done something wrong already and want the player out of there

            //if (!startNode.traversable) {Debug.LogWarning("Pathfinder Error: Start node is not traversable"); return null; }
            if(!goalNode.traversable) { Debug.LogWarning("Pathfinder Error: Goal node is not traversable"); return null; }
            if (startNode == goalNode) { Debug.LogWarning("Pathfinder Error: Startnode == goal node"); return null; }

            this.graph = graph;
            this.startNode = startNode;
            this.goalNode = goalNode;

            frontierNodes = new PriorityQueue<IPathfindingNode>();
            frontierNodes.Enqueue(startNode);

            exploredNodes = new List<IPathfindingNode>();
            pathNodes = new List<IPathfindingNode>();

            for (int x = 0; x < graph.width; x++)
            {
                for (int y = 0; y < graph.height; y++)
                {
                    graph.map[x, y].Reset();
                }
            }

            isComplete = false;
            iterations = 0;

            startNode.distanceTravelled = 0;

            SearchRoutine();

            if (DEBUG_showPath)
            {
                IPathfindingNode previous = pathNodes[0];

                foreach (IPathfindingNode cell in pathNodes)
                {
                    if (cell == previous) { continue; }

                    graph.GetXY(previous, out int x, out int y);
                    graph.GetXY(cell, out int _x, out int _y);

                    previous = cell;

                    Debug.DrawLine(graph.GetWorldPositionCentered(x, y), graph.GetWorldPositionCentered(_x, _y), DEBUG_Colour, DEBUG_duration, false);
                }
            }

            return pathNodes;
        }

        public void SearchRoutine()
        {
            float timeStart = Time.realtimeSinceStartup;

            while (!isComplete)
            {
                if (frontierNodes.Count > 0)
                {
                    IPathfindingNode currentNode = frontierNodes.Dequeue();
                    iterations++;

                    if (!exploredNodes.Contains(currentNode)) { exploredNodes.Add(currentNode); };

                    switch (mode)
                    {
                        case Mode.BreadthFirstSearch:
                            ExpandFrontierBFS(currentNode);
                            break;
                        case Mode.Dijkstra:
                            ExpandFrontierDijkstra(currentNode);
                            break;
                        case Mode.GreedyBestFirstSearch:
                            ExpandFrontierGBFS(currentNode);
                            break;
                        case Mode.AStar:
                            ExpandFrontierAStar(currentNode);
                            break;
                    }

                    if (frontierNodes.Contains(goalNode))
                    {
                        pathNodes = GetPathNodes(goalNode);

                        isComplete = true;
                    }
                }
                else
                {
                    isComplete = true;
                }
            }
            //Debug.Log("Time Elapsed: " + (Time.realtimeSinceStartup - timeStart).ToString() + " seconds");
        }

        #region ExpandFrontiers

        void ExpandFrontierBFS(IPathfindingNode node)
        {
            if (node == null) { return; }

            for (int i = 0; i < node.neighbours.Count; i++)
            {
                IPathfindingNode n = node.neighbours[i];

                if (exploredNodes.Contains(n) || frontierNodes.Contains(n)) { continue; }

                float distance = PathfinderUtils.GetNodeDistance(node, n);
                float newDistanceTravelled = distance + node.distanceTravelled + n.movementDifficulty;

                n.distanceTravelled = newDistanceTravelled;

                n.previous = node;
                n.priority = exploredNodes.Count;
                frontierNodes.Enqueue(n);
            }
        }

        void ExpandFrontierDijkstra(IPathfindingNode node)
        {
            if (node == null) { return; }

            for (int i = 0; i < node.neighbours.Count; i++)
            {
                IPathfindingNode n = node.neighbours[i];

                if (exploredNodes.Contains(n)) { continue; }

                float distance = PathfinderUtils.GetNodeDistance(node, n);
                float newDistanceTravelled = distance + node.distanceTravelled + n.movementDifficulty;

                if (float.IsPositiveInfinity(n.distanceTravelled) || newDistanceTravelled < n.distanceTravelled)
                {
                    n.previous = node;
                    n.distanceTravelled = newDistanceTravelled;
                }

                if (!frontierNodes.Contains(n))
                {
                    n.priority = n.distanceTravelled;
                    frontierNodes.Enqueue(n);
                }
            }
        }

        void ExpandFrontierGBFS(IPathfindingNode node)
        {
            if (node == null) { return; }

            for (int i = 0; i < node.neighbours.Count; i++)
            {
                IPathfindingNode n = node.neighbours[i];

                if (exploredNodes.Contains(n) || frontierNodes.Contains(n)) { continue; }

                float distance = PathfinderUtils.GetNodeDistance(node, n);
                float newDistanceTravelled = distance + node.distanceTravelled + n.movementDifficulty;

                n.distanceTravelled = newDistanceTravelled;

                n.previous = node;
                n.priority = PathfinderUtils.GetNodeDistance(n, goalNode);
                frontierNodes.Enqueue(n);
            }
        }

        void ExpandFrontierAStar(IPathfindingNode node)
        {
            if (node == null) { return; }

            for (int i = 0; i < node.neighbours.Count; i++)
            {
                IPathfindingNode n = node.neighbours[i];

                if (exploredNodes.Contains(n)) { continue; }

                float distance = PathfinderUtils.GetNodeDistance(node, n);
                float newDistanceTravelled = distance + node.distanceTravelled + n.movementDifficulty;

                if (float.IsPositiveInfinity(n.distanceTravelled) || newDistanceTravelled < n.distanceTravelled)
                {
                    n.previous = node;
                    n.distanceTravelled = newDistanceTravelled;
                }

                if (!frontierNodes.Contains(n))
                {
                    float distanceToGoal = PathfinderUtils.GetNodeDistance(n, goalNode);
                    n.priority = n.distanceTravelled + distanceToGoal;
                    frontierNodes.Enqueue(n);
                }
            }
        }

        #endregion

        List<IPathfindingNode> GetPathNodes(IPathfindingNode endNode)
        {
            List<IPathfindingNode> path = new List<IPathfindingNode>();

            if (endNode == null) { return path; }

            path.Add(endNode);

            IPathfindingNode currentNode = endNode.previous;

            while (currentNode != null)
            {
                path.Insert(0, currentNode);
                currentNode = currentNode.previous;
            }

            return path;
        }
    }

    public interface IPathfindingNode : System.IComparable<IPathfindingNode>, System.IEquatable<IPathfindingNode>
    {
        int xIndex { get; set; }
        int yIndex { get; set; }
        Vector3 position { get; set; }

        float movementDifficulty { get; set; }
        bool traversable { get; }
        float distanceTravelled { get; set; }
        float priority { get; set; }
        IPathfindingNode previous { get; set; }
        List<IPathfindingNode> neighbours { get; }

        void Reset();
    }

    
}