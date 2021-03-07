using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWH.Grid
{
    public static class GridUtils
    {
        public static T GetGridValueAtMousePos<T>(GenericGrid<T> grid, Camera camera) where T : System.IEquatable<T>
        {
            Vector2 mousePosition = Input.mousePosition;

            Ray mouseRay = camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

            Physics.Raycast(mouseRay, out RaycastHit hit);

            grid.GetXY(hit.point, out int x, out int y);

            return grid.GetValue(x,y);
        }
    }
}