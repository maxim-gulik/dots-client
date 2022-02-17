using Dots.AC;
using DotsGame.EndlessMode;
using UnityEngine;

namespace DotsGame.Common
{
    /// <summary>
    /// Responsible for building and drawing the grid.
    /// Contains convenient API to work convert grid coords to world positions etc
    /// </summary>
    public class GridActor : BaseActor
    {
        private GridCell[][] _gridMatrix;
        private float _cellSize;

        public void Construct(GridSettings gridSettings)
        {
            Rows = gridSettings.Rows;
            Columns = gridSettings.Columns;

            _cellSize = gridSettings.CalcCellSide();

            _gridMatrix = BuildGridMatrix();
        }

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public Vector2 GetPosByCellCoords(Vector2Int coords) => _gridMatrix[coords.x][coords.y].ContentPos;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnDrawGizmos()
        {
            if (_gridMatrix == null)
                return;
            
            for (var c = 0; c < _gridMatrix.Length; c++)
            for (var r = 0; r < _gridMatrix[c].Length; r++)
            {
                Gizmos.DrawSphere(_gridMatrix[c][r].ContentPos, 0.025f);
            }
        }

        private GridCell[][] BuildGridMatrix()
        {
            var matrix = new GridCell[Columns][];
            for (var c = 0; c < Columns; c++)
            {
                var column = new GridCell[Rows];
                for (var r = 0; r < Rows; r++)
                    column[r] = CreateGridCell(c, r);

                matrix[c] = column;
            }

            return matrix;
        }

        private GridCell CreateGridCell(int x, int y)
        {
            var position = CalcPosByCellCoords(x, y);
            var coords = new Vector2Int(x, y);
            return new GridCell(coords, position);
        }

        private Vector2 CalcPosByCellCoords(int x, int y)
        {
            var zeroPosition = new Vector2(
                -_cellSize * ((Columns - 1) / 2f),
                -_cellSize * ((Rows - 1) / 2f));

            return new Vector2(
                zeroPosition.x + _cellSize * x,
                zeroPosition.y + _cellSize * y);
        }

        public class GridCell
        {
            public readonly Vector2Int Coords;
            public readonly Vector2 ContentPos;

            public GridCell(Vector2Int coords, Vector2 contentPos)
            {
                Coords = coords;
                ContentPos = contentPos;
            }
        }
    }
}