
The main files handling all logic are Pathfinder.cs and GenercGrid.cs

To create a grid simply use the PWH.Grid namespace and create a

new GenericGrid<T>()

The T in the Generic Grid Tells it what data it will be holding (similar to List<int>),
for example of you make a GenericGrid<int>(5,5), you will end up with an array like so:
00000
00000
00000
00000
00000

If you for example wanted to change one of these ints into a 5, you would do like so:

grid.SetValue(4,4,5); (Remember that a grid starts at (0,0) so the top right corner is (4,4) not (width,height))

This would result in:

00005
00000
00000
00000
00000

However there is one more step in setting up a Grid. You need to give it a function that returns the type you want it to hold.
(https://docs.microsoft.com/en-us/dotnet/api/system.func-2?view=net-5.0, useful for understanding the third arg)

||||
new GenericGrid<int>(10, 10, (GenericGrid<int> source, int x, int y) => GenerateInt(source, x, y));

int GenerateInt(GenericGrid<int> source,int x,int y)
{
	return Random.Range(0, 5);
}
|||||
See the above example ^

In this example, we create a function which returns an int, the func arg passes this to the Grid, 
and the grid calls this method for every single space to give fill it with default values

There is a 4th optional arg (excluding the arguments after it which are just debug) which is cell size,
this arg isn't useful if you aren't using the grid in a world space setting, however if you are
then this dictates what functions like GetWorldPosition return. 
(Grids always start at (0,0), so if a cell size is 1 then GetWorldPosition(1,1) is also (1,1) in the world)

The pathfinder is a singleton monobehavior which requires:

A generic grid of IPathfindingNodes, a Start IPathfindingNode (From the same grid) and a goalNode.

Since the first argument is an interface, you cannot actually have a grid of them of course,
so make a class that inherits from IPathfindingNode and create a Grid From that (I provide one in the Examples Folder)

Next, use the PathfinderUtils (found in the namespace PWH.Grid.Pathfinding) to

ConvertGridToGraph(GenericGrid<T> grid)

This takes any grid (make sure the objects inside it inherit from IPathfindingNode) and returns a new
GenericGrid of type IPathfindingNode (this will be further referred to as a "graph")

(Please check PathGridObject.cs included in examples for an example on how to setup a GridObject that inherits from IPathfindingNode)
(You can also just make your own GridObject inherit from PathGridObject and have to do no setup)

Once you have your graph, simply use .GetValue() to get your startNode and your goalNode and pass them into the PathFinder like so:

Pathfinder.instance.Init(graph,startNode,goalNode);

This function returns a list of IPathfindingNodes in order from start to goal.

(Useful function: GridUtil.cs contains a function for getting the GridCell at your mouse position)

Checkout GridHolder.cs in Examples for a more indepth explanation with code
