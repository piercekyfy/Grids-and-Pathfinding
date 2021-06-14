using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PWH.Grids.Pathfinding;

namespace PWH.Grids.Examples
{
    public class HexagonGridHolder : MonoBehaviour
    {
        // Remember grids start at 0,0 so a width and height of 5 will result in the top right corner being 4,4 on the grid.
        public int width = 5;
        public int height = 5;
        public float cellSize = 10f;
        public bool doDebug = true;

        HexagonGrid<GridCell> grid;
        Pathfinder pathfinder;

        private void Start()
        {
            pathfinder = Pathfinder.instance;

            // When Initializing a grid you need to provide it with a function that returns an instance of whatever type
            // of object the grid holds.

            grid = new HexagonGrid<GridCell>(GridAxis.XZ, Vector2.zero, width, height,
                (Grid<GridCell> source, int x, int y) => GenerateNewGridCell(source, x, y)
                ,cellSize, doDebug, 40, 0.075f);

            // The key feature of a Hexagon grid, is that it can generate it's own mesh
            // I plan to add more features to this mesh generation (from following CatLikeCoding's course, highly recommended!)
            grid.GenerateMesh(GetComponent<MeshFilter>(), GetComponent<MeshCollider>());
        }

        public void PathfindTo(int x, int y)
        {
            if (!pathfinder) { Debug.LogError("No pathfinder"); return; }

            // The pathfinder takes an Grid of IPathfindingNodes.
            // You can convert any grid of objects that inherit from IPathfindingNode to a
            // Grid of IPathfindingNodes via the PathfindingUtils class

            Grid<IPathfindingNode> graph = PathfinderUtils.ConvertGridToGraph(grid);

            IPathfindingNode startNode = graph.GetValue(0, 0);
            IPathfindingNode endNode = graph.GetValue(x, y);

            float debugDuration = 1f;

            List<IPathfindingNode> path = pathfinder.Init(graph, startNode, endNode, true, debugDuration);

            // The pathfinder returns the path to the goal node in order from start to goal.
            // You can reverse engineer it with graph.GetWorldPositionCentered() to move something along it
            // I recommend DoTween for this
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GridCell cell = GridUtils.GetGridValueAtMousePos(grid, Camera.main, out bool foundValue);

                if (foundValue)
                {
                    grid.GetXY(cell, out int x, out int y);
                    

                    PathfindTo(x, y);
                }
            }
        }

        // Randomises the cell's movement difficulty in the grid between passable (1) and impassable (0)
        GridCell GenerateNewGridCell(Grid<GridCell> source, int x, int y)
        {
            // This randomisation is commented out so that the Pathfinder works on first try. To preview it simply remove
            // the 1 and /* */
            int movementDifficulty = Random.Range(/*0*/1, 2);
            return new GridCell(source, x, y, movementDifficulty);
        }
    }
}


