using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWH.Grids
{
    public class HexagonGrid<T> : Grid<T>
    {
        public Vector3[] meshCorners { get; protected set; }

        public float innerRadius = 0f;

        Mesh mesh = new Mesh();
        List<Vector3> verticles = new List<Vector3>();
        List<int> triangles = new List<int>();

        Dictionary<HexagonCorner, Vector3> cornerToPoint;

        public HexagonGrid(GridAxis gridAxis, Vector3 offset, int width, int height, System.Func<Grid<T>, int, int, T> createGridObjectFunc, float cellSize, bool showDebug = false, int debugFontSize = 40, float debugFontScale = 0.1f) : base(gridAxis, offset, width, height, createGridObjectFunc, cellSize)
        {
            innerRadius = cellSize * 0.866025404f; // * sqrt(2)
            mesh.name = "Hexagon Grid";

            if(gridAxis == GridAxis.XY)
            {
                meshCorners = new Vector3[]
                {
                    new Vector3(0, cellSize, 0),
                    new Vector3(innerRadius, 0.5f * cellSize, 0),
                    new Vector3(innerRadius, -0.5f * cellSize, 0),
                    new Vector3(0f, -cellSize, 0),
                    new Vector3(-innerRadius, -0.5f * cellSize, 0),
                    new Vector3(-innerRadius, 0.5f * cellSize, 0),
                    new Vector3(0f, cellSize, 0)
                };
            }
            else
            {
                meshCorners = new Vector3[]
                {
                    new Vector3(0f, 0f, cellSize), //0
                    new Vector3(innerRadius, 0f, 0.5f * cellSize), //1
                    new Vector3(innerRadius, 0f, -0.5f * cellSize), //2
                    new Vector3(0f, 0f, -cellSize), // 3
                    new Vector3(-innerRadius, 0f, -0.5f * cellSize), //4
                    new Vector3(-innerRadius, 0f, 0.5f * cellSize), //5
                    new Vector3(0f, 0f, cellSize)
                };
            }
            
            cornerToPoint = new Dictionary<HexagonCorner, Vector3>()
            {
                { HexagonCorner.Top, meshCorners[0] },
                { HexagonCorner.TopRight, meshCorners[1] },
                { HexagonCorner.BottomRight, meshCorners[2] },
                { HexagonCorner.Bottom, meshCorners[3] },
                { HexagonCorner.BottomLeft, meshCorners[4] },
                { HexagonCorner.TopLeft, meshCorners[5] },
            };

            if (showDebug) { ShowDebug(debugFontSize, debugFontScale); };
        }
        
        public override float GetCellDistance(int x1, int y1, int x2, int y2)
        {
            int dx = x2 - x1;     // signed deltas
            int dy = y2 - y1;
            int x = Mathf.Abs(dx);  // absolute deltas
            int y = Mathf.Abs(dy);
            // special case if we start on an odd row or if we move into negative x direction
            if ((dx < 0) ^ ((y1 & 1) == 1))
                x = Mathf.Max(0, x - (y + 1) / 2);
            else
                x = Mathf.Max(0, x - (y) / 2);
            return x + y;
        }

        public override List<T> GetCellNeighbors(int x, int y, List<Vector2Int> directions = null)
        {
            if (directions != null)
            {
                return base.GetCellNeighbors(x, y, directions);
            }

            directions = new List<Vector2Int>()
            {
                new Vector2Int(-1,0),
                new Vector2Int(1,0),
                new Vector2Int(y % 2, 1),
                new Vector2Int((y % 2) - 1, 1),
                new Vector2Int(y % 2, -1),
                new Vector2Int((y % 2) - 1, -1)
            };

            return base.GetCellNeighbors(x, y, directions);
        }

        public void GenerateMesh(MeshFilter meshFilter, MeshCollider meshCollider = null)
        {
            mesh.Clear();
            verticles.Clear();
            triangles.Clear();

            meshFilter.mesh = mesh;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Triangulate(GetWorldPosition(x, y));
                }
            }

            mesh.vertices = verticles.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            if (meshCollider)
                meshCollider.sharedMesh = mesh;
        }

        void Triangulate(Vector3 position)
        {
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(
                    position,
                    position + meshCorners[i],
                    position + meshCorners[i + 1]
                );
            }
        }

        void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = verticles.Count;
            verticles.Add(v1);
            verticles.Add(v2);
            verticles.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        public Vector3 GetWorldPosition(int x, int y, HexagonCorner corner)
        {
            return GetWorldPosition(x,y) + cornerToPoint[corner];
        }

        // Centered has no effect, as this is already centered.
        public override Vector3 GetWorldPosition(int x, int y, bool centered = false)
        {
            if(Gridaxis == GridAxis.XY)
                return new Vector3((x + y * 0.5f - y / 2) * (innerRadius * 2f), y * (WorldSpaceCellSize * 1.5f), WorldSpaceOffset.z);
            else
                return new Vector3((x + y * 0.5f - y / 2) * (innerRadius * 2f), WorldSpaceOffset.y, y * (WorldSpaceCellSize * 1.5f));
        }

        TextMesh[,] textMeshMap;

        public void ShowDebug(int fontSize, float debugFontScale = 0.1f)
        {
            textMeshMap = new TextMesh[Map.GetLength(0), Map.GetLength(1)];

            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    HexagonCorner lastCorner = HexagonCorner.Top;
                    foreach(HexagonCorner corner in System.Enum.GetValues(typeof(HexagonCorner)))
                    {
                        Debug.DrawLine(GetWorldPosition(x,y,lastCorner),GetWorldPosition(x, y, corner),Color.black,9999f,false);
                        lastCorner = corner;

                    }

                    GameObject valueText = new GameObject();
                    valueText.name = "DebugText (" + x + "," + y + ")";
                    valueText.transform.position = GetWorldPosition(x, y, true) + new Vector3(0, 0.2f, 0);
                    valueText.transform.eulerAngles = Gridaxis == GridAxis.XZ ? new Vector3(90, 0, 0) : new Vector3(0,0,0);
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

            GridValueChanged += (object sender, GridValueChangedEventArgs args) => { textMeshMap[args.x, args.y].text = GetValue(args.x, args.y)?.ToString(); };
        }
    }

    public enum HexagonCorner
    {
        Top,
        TopLeft,
        BottomLeft,
        Bottom,
        BottomRight,
        TopRight
    }
}
