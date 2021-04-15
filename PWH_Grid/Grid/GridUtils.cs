using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWH.Grid
{
    // The grid needs to be ontop of a surface with a collider as it uses raycasts

    public static class GridUtils
    {
        public static T GetGridValueAtMousePos<T>(GenericGrid<T> grid, Camera camera,out bool foundValue)
        {
            Vector2 mousePosition = Input.mousePosition;

            Ray mouseRay = camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

            if(Physics.Raycast(mouseRay, out RaycastHit hit))
            {
                grid.GetXY(hit.point, out int x, out int y);

                T cell = grid.GetValue(x, y, out bool _foundValue);

                foundValue = _foundValue;

                return cell;
            } else
            {
                foundValue = false;
                return default;
            }

            
        }
    }
}