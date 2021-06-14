using System.Collections.Generic;
using UnityEngine;

namespace PWH.Grids
{
    public class SquareGrid<T> : Grid<T>
    {
        TextMesh[,] textMeshMap;
        public bool ConnectDiagonally { get; set; }

        public SquareGrid(GridAxis gridAxis, Vector3 offset, int width, int height, System.Func<Grid<T>, int, int, T> createGridObjectFunc, float cellSize,bool connectDiagonally, bool showDebug = false, int debugFontSize = 40, float debugFontScale = .1f) : base(gridAxis, offset, width, height, createGridObjectFunc, cellSize)
        {
            this.ConnectDiagonally = connectDiagonally;
            if (showDebug) { ShowDebug(debugFontSize, debugFontScale); }
        }

        public override float GetCellDistance(int x1, int y1, int x2, int y2)
        {
            int dx = Mathf.Abs(x1 - x2);
            int dy = Mathf.Abs(y1 - y2);

            int min = Mathf.Min(dx, dy);
            int max = Mathf.Max(dx, dy);

            int diagonalSteps = min;
            int straightSteps = max - min;

            return (1.4f * diagonalSteps + straightSteps);
        }

        public override List<T> GetCellNeighbors(int x, int y, List<Vector2Int> directions = null)
        {
            if(directions != null)
                return base.GetCellNeighbors(x, y, directions);

            if (ConnectDiagonally)
            {
                directions = new List<Vector2Int>()
                {
                    new Vector2Int(0,1), // UP
                    new Vector2Int(0,-1), // DOWN
                    new Vector2Int(1,0), // RIGHT
                    new Vector2Int(-1,0), // LEFT
                    new Vector2Int(1,1), // UP RIGHT
                    new Vector2Int(-1,1), // UP LEFT
                    new Vector2Int(1,-1), // DOWN RIGHT
                    new Vector2Int(-1,-1), // DOWN LEFT
                };
                
            }
            else
            {
                directions = new List<Vector2Int>()
                {
                    new Vector2Int(0,1), // UP
                    new Vector2Int(0,-1), // DOWN
                    new Vector2Int(1,0), // RIGHT
                    new Vector2Int(-1,0), // LEFT
                };
            }

            return base.GetCellNeighbors(x, y, directions);
        }

        public void RedrawDebug()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    textMeshMap[x, y].text = Map[x, y].ToString();
                }
            }
        }

        public void RedrawDebug(int x,int y)
        {
            textMeshMap[x, y].text = Map[x, y].ToString();
        }

        private void ShowDebug(int fontSize, float debugFontScale)
        {
            textMeshMap = new TextMesh[Map.GetLength(0), Map.GetLength(1)];

            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.black, 9999f, false);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.black, 9999f, false);

                    GameObject valueText = new GameObject();
                    valueText.name = "DebugText (" + x + "," + y + ")";
                    valueText.transform.position = GetWorldPosition(x, y, true) + new Vector3(0, 0.1f, 0);
                    valueText.transform.eulerAngles = Gridaxis == GridAxis.XZ ? new Vector3(90, 0, 0) : new Vector3(0, 0, 0);
                    valueText.transform.localScale = new Vector3(debugFontScale, debugFontScale, debugFontScale);
                    TextMesh textMesh = valueText.AddComponent<TextMesh>();
                    textMesh.fontSize = fontSize;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.text = Map[x, y]?.ToString();
                    textMesh.color = Color.black;
                    textMesh.alignment = TextAlignment.Center;

                    textMeshMap[x, y] = textMesh;
                }
            }

            Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.black, 9999f, false);
            Debug.DrawLine(GetWorldPosition(Width, 0), GetWorldPosition(Width, Height), Color.black, 9999f, false);

            GridValueChanged += (object sender, GridValueChangedEventArgs args) => { textMeshMap[args.x, args.y].text = GetValue(args.x, args.y)?.ToString(); };
        }
    }
}