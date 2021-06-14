


# !GRIDS AND PATHFINDING!

## Demo Videos

### The Square Grid with Pathfinding
https://user-images.githubusercontent.com/80215001/121910671-f3f05f80-cd26-11eb-8b0a-b931839177ee.mp4

can be located at Grids&Pathfinding/Examples/SquareGrid.unity

### The Hexagon Grid
https://user-images.githubusercontent.com/80215001/121910619-e9ce6100-cd26-11eb-842a-32eb7ca3288f.mp4

can be located at Grids&Pathfinding/Examples/HexagonGrid.unity

## Example of Use

### Explanation

This example will create a new HexagonGrid<T> and generate a mesh for it.
  
All grids need 6 arguments (and 1 type) for their basic functions.
  
Grids are generic, so they require a Type. This is the Type that the grid will contain:
  
`new HexagonGrid<int>(` <- A Hexagon Grid which cells are integers
                           
The **first argument** or creating a grid is the Axis it will exist on, this is passed as the GridAxis enum:
                           
`new HexagonGrid<int>(GridAxis.XY` creates a grid on the X and Y axis, 
  
replacing it with `GridAxis.XZ` will create a grid on the X and Z axis.

The **second argument** is the World Space offset of the grid, this effects any functions which reurn the world space position of a cell,

If `new Vector3(0, 5, 0)` is passed, then the [0,0] cell of the grid will be at the world position of (0, 5, 0). 
  
Keep in mind the [0,0] cell of a grid is the bottom left.
  
The **third and fourth arguments** are the width and height, remember that a grid starts at [0,0], so if you give it a width and height of 6 and 6, the top right corner will be [5,5]
  
The **fifth argument** is a Func, which gets a Grid<T> argument, an int for the x and an int for the y and returns an instance of T. This function is used to initialize your grid, it will take the return of it and use it to fill every spot in your grid.

The **sixth argument** is cellSize, which is another that only matters in the worldSpace operations.
  
Hexagon and Square grids have 3 additional Debug arguments: showDebug, debugFontSize and debugFontScale.
  
### Final Code
  
```
using PWH.Grids;
HexagonGrid<int> hexGrid = 
  new HexagonGrid<int>
  (
    GridAxis.XY, // Grid Axis
    new Vector3(0, 10, 0), // Offset
    6, // Width
    6, // Height
    (source, x, y) => { return 1; }, // Cell Initialization Func
    10f, // Cell Size
    true, // Do Debug?
    40, // Debug Font Size
    0.075f// Debug Font Scale
); 
```
To generate a mesh with your newly created HexagonGrid, simply call:
`hexGrid.GenerateMesh(GetComponent<MeshFilter>(), GetComponent<MeshCollider>())`

## Pathfinding
### See Grids&Pathfinding/Examples/PathGridObject on how to implement IPathfindingNode and setup your GridCell Object to work with pathfinding!

The PathFinder is the powerhouse of all pathfinding, it is a Singleton which simply needs to be placed in your scene, from there you can get a List<IPathfindingNodes> from calling PathFinder.Instance.Init().
  
This List is the path from your specified start to your specified goal in order.

It takes a Grid<IPathfindingNode> as it's **first argument**, if you have a Grid of elements that inherit from IPathfinding Node, simply give it to the utility function:
  PathfinderUtils.ConvertGridToGraph<T>(Your Grid), and it will return a Grid<IPathfindingNode> which you can give to the pathFinder.
  
From there you simply get your first node and goal node and pass them into the .Init() as the **second and third argument** respectively.
  
There is also a **third argument** which is a Debug option to show the path it generated in red.
  
**The Pathfinder has 4 modes:** AStar, GBFS, Dijkstra and BFS, this mode can be set in the inspector or by the `PathFinder.Instance.mode =` field.
