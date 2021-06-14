using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWH.Grids
{
    public static class GridUtils
    {
        // The grid needs to be ontop of a surface with a collider as this uses raycasts
        public static T GetGridValueAtMousePos<T>(Grid<T> grid, Camera camera, out bool foundValue)
        {
            Vector2 mousePosition = Input.mousePosition;

            Ray mouseRay = camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

            if(Physics.Raycast(mouseRay, out RaycastHit hit))
            {
                grid.GetXY(hit.point, out int x, out int y);

                T cell;
                try
                {
                   cell = grid.GetValue(x, y);
                }
                catch(System.ArgumentException)
                {
                    foundValue = false;
                    return default;
                }

                foundValue = true;
                return cell;
            } else
            {
                foundValue = false;
                return default;
            }
        }
    }
}
