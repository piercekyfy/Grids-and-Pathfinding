using System.Collections.Generic;
using UnityEngine;
using PWH.Grid.Pathfinding;

namespace PWH.Grid.Examples
{
    // GridObject that implements the pathfinding node interface, needs another class to inherit from it

    public abstract class PathGridObject<T> : IPathfindingNode, System.IEquatable<PathGridObject<T>> where T : PathGridObject<T>, System.IEquatable<T>
    {
        public GenericGrid<T> sourceGrid { get; set; }

        public int xIndex { get; set; }
        public int yIndex { get; set; }
        public Vector3 position { get; set; }
        public float movementDifficulty { get; set; }

        public virtual bool traversable => movementDifficulty == 0 ? false : true;

        public float distanceTravelled { get; set; } = Mathf.Infinity;
        public float priority { get; set; }
        public IPathfindingNode previous { get; set; }
        public List<IPathfindingNode> neighbours => PathfinderUtils.FilterTraversable(PathfinderUtils.ToIPathfindingNodes(sourceGrid.GetNeighbors(xIndex, yIndex, PathfinderUtils.uniformDirections)));

        public PathGridObject(GenericGrid<T> sourceGrid, int xIndex, int yIndex, float movementDifficulty)
        {
            this.sourceGrid = sourceGrid;

            // Pathfinding Stuff
            this.xIndex = xIndex;
            this.yIndex = yIndex;
            this.movementDifficulty = movementDifficulty;
            position = sourceGrid.GetWorldPosition(xIndex, yIndex);
        }

        public void Reset()
        {
            previous = null;
            distanceTravelled = Mathf.Infinity;
        }

        public int CompareTo(IPathfindingNode other)
        {
            if (priority < other.priority)
            {
                return -1;
            }
            else if (this.priority > other.priority)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public bool Equals(PathGridObject<T> other)
        {
            return (xIndex == other.xIndex && yIndex == other.yIndex);
        }

        public bool Equals(IPathfindingNode other)
        {
            return (xIndex == other.xIndex && yIndex == other.yIndex);
        }

        public override string ToString()
        {
            return "M Dif: " + movementDifficulty + "\nPos: (" + xIndex + "," + yIndex + ")";
        }
    }
}


