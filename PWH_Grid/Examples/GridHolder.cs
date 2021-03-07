using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PWH.Grid.Pathfinding;

namespace PWH.Grid.Examples
{
    public class GridHolder : MonoBehaviour
    {
        GenericGrid<PathGridObject> grid;

        private void Start()
        {
            // Remember grids start at 0,0 so a width and height of 5 will result in the top right corner being 4,4 on the grid.
            int width = 5;
            int height = 5;
            float cellSize = 1;
            bool doDebug = true;
            float debugTextScale = 0.05f;
            int debugTextFontSize = 40;

            // When Initializing a grid you need to provide it with a function that returns an instance of whatever type
            // of object the grid holds.

            grid = new GenericGrid<PathGridObject>(width, height,
                (GenericGrid<PathGridObject> source, int x, int y) => GenerateNewPathGridObject(source,x,y)
                ,cellSize,doDebug,debugTextScale,debugTextFontSize);

            Pathfinder pathfinder = Pathfinder.instance;
            
            if(!pathfinder) { return; }

            // The pathfinder takes an GenericGrid of IPathfindingNodes.
            // You can convert any grid of objects that inherit from IPathfindingNode to a
            // Generic grid of IPathfindingNodes via the PathfindingUtils class

            GenericGrid<IPathfindingNode> graph = PathfinderUtils.ConvertGridToGraph(grid);

            IPathfindingNode startNode = graph.GetValue(0, 0);
            IPathfindingNode endNode = graph.GetValue(4, 4);

            float debugDuration = 10f;

            List<IPathfindingNode> path = pathfinder.Init(graph, startNode, endNode, doDebug, debugDuration);
            
            // The pathfinder returns the path to the goal node in order from start to goal.
            // You can reverse engineer it with graph.GetWorldPositionCentered() to move something along it
            // I recommend DoTween Sequences for this
        }

        // Randomises the cell's movement difficulty in the grid between passable (1) and impassable (0)
        PathGridObject GenerateNewPathGridObject(GenericGrid<PathGridObject> source,int x,int y)
        {
            // This randomisation is commented out so that the Pathfinder works on first try. To preview it simply remove
            // the 1 and /* */
            int movementDifficulty = Random.Range(/*0*/1, 2);
            return new PathGridObject(source, x, y, movementDifficulty);
        }
    }
}


