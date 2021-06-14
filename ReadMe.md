


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
