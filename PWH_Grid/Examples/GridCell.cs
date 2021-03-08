using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWH.Grid.Examples
{
    // Since PathGridObject is abstract, we make inherit it in this script, which does nothing extra.

    public class GridCell : PathGridObject<GridCell>, System.IEquatable<GridCell>
    {
        public GridCell(GenericGrid<GridCell> sourceGrid, int xIndex, int yIndex, float movementDifficulty) : base(sourceGrid, xIndex, yIndex, movementDifficulty)
        {
        }

        public bool Equals(GridCell other)
        {
            return (xIndex == other.xIndex && yIndex == other.yIndex);
        }
    }
}

