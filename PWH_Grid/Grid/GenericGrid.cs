using System.Collections.Generic;
using UnityEngine;

namespace PWH.Grid
{
    public class GenericGrid<T>
    {
        public Vector3 offset { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        public float cellSize { get; private set; }

        public T[,] map { get; private set; }
        public Dictionary<T, Vector2Int> coordMap { get; private set; }

        event System.EventHandler<GridValueChangedEventArgs> GridValueChanged;

        TextMesh[,] textMeshMap;


        public GenericGrid(Vector2 position,int width, int height, System.Func<GenericGrid<T>, int, int, T> createGridObject, float cellSize = 1f, bool showDebug = false, float textScale = 0.1f, int textFontSize = 40)
        {
            this.offset = position;
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            map = new T[width, height];
            coordMap = new Dictionary<T, Vector2Int>();

            // Init Values with the provided Func

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    T newObject = createGridObject(this, x, y);
                    map[x, y] = newObject;
                    coordMap.Add(newObject, new Vector2Int(x, y));
                }
            }

            //

            if (showDebug) { ShowDebug(width, height, textScale, textFontSize); }
        }

        #region Grid Interaction

        public void SetValue(int x, int y, T value)
        {
            T cell = GetValue(x, y, out bool foundValue);
            if (foundValue)
            {
                coordMap.Remove(cell);
            }
            
            map[x, y] = value;

            coordMap.Add(value, new Vector2Int(x, y));

            OnValueChanged(x, y);
        }

        public void SetValue(Vector3 position, T value)
        {
            int x, y;
            GetXY(position, out x, out y);

            SetValue(x, y, value);
        }

        public T GetValue(int x, int y,out bool foundValue)
        {
            if (WithinBounds(x, y))
            {
                foundValue = true;
                return map[x, y];
            }
            else
            {
                foundValue = false;
                return default;
            }
        }

        public void OnValueChanged(int x, int y)
        {
            GridValueChanged?.Invoke(this, new GridValueChangedEventArgs(x, y));
        }

        #endregion

        #region Utils

        public bool WithinBounds(int x, int y)
        {
            if ((x < 0f || y < 0f) || (x > map.GetLength(0) - 1 || y > map.GetLength(1) - 1)) { return false; }
            return true;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x + offset.x, 0, y + offset.y) * cellSize;
        }

        public Vector3 GetWorldPositionCentered(int x, int y)
        {
            return new Vector3(x + offset.x, 0, y + offset.y) * cellSize + new Vector3(cellSize / 2, 0, cellSize / 2);
        }

        public Vector3 GetWorldPosition(T cell)
        {
            GetXY(cell, out int x, out int y);

            return new Vector3(x + offset.x, 0, y + offset.y) * cellSize;
        }

        public Vector3 GetWorldPositionCentered(T cell)
        {
            GetXY(cell, out int x, out int y);

            return new Vector3(x + offset.x, 0, y + offset.y) * cellSize + new Vector3(cellSize / 2, 0, cellSize / 2);
        }

        public void GetXY(T cell, out int x, out int y)
        {
            if(coordMap.TryGetValue(cell, out Vector2Int value))
            {
                x = value.x;
                y = value.y;
            } else
            {
                x = -1;
                y = -1;
            }

            /*
            for (int _x = 0; _x < map.GetLength(0); _x++)
            {
                for (int _y = 0; _y < map.GetLength(1); _y++)
                {
                    if (cell.Equals(map[_x, _y]))
                    {
                        x = _x;
                        y = _y;
                        return;
                    }
                }
            }
            x = -1;
            y = -1;
            */
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            worldPosition.x -= offset.x;
            worldPosition.z -= offset.y;
            x = Mathf.FloorToInt(worldPosition.x / cellSize);
            y = Mathf.FloorToInt(worldPosition.z / cellSize);
        }

        public List<T> GetNeighbors(int x, int y, Vector2[] directions)
        {
            List<T> neighbors = new List<T>();

            foreach (Vector2 dir in directions)
            {
                int newX = x + (int)dir.x;
                int newY = y + (int)dir.y;

                if (WithinBounds(newX, newY))
                {
                    neighbors.Add(map[newX, newY]);
                }
            }

            return neighbors;
        }

        #endregion

        public void RedrawDebug()
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    textMeshMap[x, y].text = map[x, y].ToString();
                }
            }
        }

        public void RedrawDebug(int x,int y)
        {
            textMeshMap[x, y].text = map[x, y].ToString();
        }

        private void ShowDebug(int width, int height, float textScale, int textFontSize)
        {
            textMeshMap = new TextMesh[map.GetLength(0), map.GetLength(1)];

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.black, 9999f, false);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.black, 9999f, false);

                    GameObject valueText = new GameObject();
                    valueText.name = "DebugText (" + x + "," + y + ")";
                    valueText.transform.position = GetWorldPositionCentered(x, y) + new Vector3(0, 0.1f, 0);
                    valueText.transform.eulerAngles = new Vector3(90, 0, 0);
                    valueText.transform.localScale = new Vector3(textScale, textScale, 0);
                    TextMesh textMesh = valueText.AddComponent<TextMesh>();
                    textMesh.fontSize = textFontSize;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.text = map[x, y]?.ToString();
                    textMesh.color = Color.black;
                    textMesh.alignment = TextAlignment.Center;

                    textMeshMap[x, y] = textMesh;
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 9999f, false);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 9999f, false);

            GridValueChanged += (object sender, GridValueChangedEventArgs args) => { textMeshMap[args.x, args.y].text = GetValue(args.x, args.y,out bool foundValue)?.ToString(); };
        }

        public class GridValueChangedEventArgs : System.EventArgs
        {
            public int x { get; set; }
            public int y { get; set; }

            public GridValueChangedEventArgs(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}