using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;
using Platform_Creator_CS.Utility;
using Point = Platform_Creator_CS.Utility.Point;

namespace Platform_Creator_CS.Sources.Entities.Containers {
    public class EntityMatrixContainer : EntityContainer {
        private List<Entity>[][] _matrixGrid;

        private int _matrixHeight = Constants.MinMatrixSize;
        private int _matrixWidth = Constants.MinMatrixSize;

        public EntityMatrixContainer() {
            _matrixGrid = new List<Entity>[_matrixWidth][];
            for (var i = 0; i < _matrixWidth; ++i)
                _matrixGrid[i] = new List<Entity>[_matrixHeight];

            AllocateMatrix();

            MatrixRect = new Rect(0f, 0f, Constants.MatrixCellSize * _matrixWidth,
                Constants.MatrixCellSize * _matrixHeight);

            UpdateActiveCells();

            OnRemoveEntity = entity => {
                foreach (var cell in entity.GridCells)
                    _matrixGrid[cell.X][cell.Y].Remove(entity);
                entity.GridCells.Clear();
            };
        }

        [JsonIgnore]
        public Rect MatrixRect { get; set; }

        [JsonIgnore]
        public Rect ActiveRect { get; set; } =
            new Rect(0f, 0f, Constants.ViewportRatioWidth, Constants.ViewportRatioHeight);

        [JsonIgnore]
        public IEnumerable<GridCell> ActiveGridCells { get; private set; }

        [JsonIgnore]
        public bool DrawDebugRects { get; set; } = true;

        public Entity FollowEntity { get; set; } = null;

        public int MatrixWidth {
            get => _matrixWidth;
            set {
                if (value >= Constants.MinMatrixSize) {
                    _matrixWidth = value;

                    Utility.Utility.Resize2DArray(ref _matrixGrid, _matrixWidth, _matrixHeight);

                    AllocateMatrix();
                }
            }
        }

        public int MatrixHeight {
            get => _matrixHeight;
            set {
                if (value >= Constants.MinMatrixSize) {
                    _matrixHeight = value;

                    Utility.Utility.Resize2DArray(ref _matrixGrid, _matrixWidth, _matrixHeight);

                    AllocateMatrix();
                }
            }
        }

        protected override IEnumerable<Entity> GetProcessEntities() {
            return GetAllEntitiesInCells(ActiveGridCells);
        }

        public override IEnumerable<Entity> FindEntitiesByTag(string tag) {
            return GetAllEntitiesInCells(ActiveGridCells).Where(e => e.Tag == tag);
        }

        public override Entity AddEntity(Entity entity) {
            PlaceEntityInMatrix(entity);

            entity.Box.OnChangedEventHandler += rect => PlaceEntityInMatrix(entity);

            return base.AddEntity(entity);
        }

        public override void Update(GameTime gameTime) {
            UpdateActiveCells();

            if (FollowEntity != null && AllowUpdatingEntities)
                ActiveRect.Position = new Point(
                    MathHelper.Clamp(FollowEntity.Box.Center().X - ActiveRect.Size.Width / 2f, MatrixRect.Left(),
                        Math.Max(0f, MatrixRect.Right() - ActiveRect.Size.Width)),
                    MathHelper.Clamp(FollowEntity.Box.Center().Y - ActiveRect.Size.Height / 2f, MatrixRect.Top(),
                        Math.Max(0f, MatrixRect.Bottom() - ActiveRect.Size.Height))
                );

            base.Update(gameTime);
        }

        public override void Render(SpriteBatch batch, float alpha) {
            base.Render(batch, alpha);

            DrawDebug(batch, alpha);
        }

        public IEnumerable<Entity> GetAllEntitiesInCells(IEnumerable<GridCell> cells) {
            var entities = new List<Entity>();

            foreach (var cell in cells) entities.AddRange(_matrixGrid[cell.X][cell.Y]);

            return new HashSet<Entity>(entities);
        }

        public IEnumerable<Entity> GetAllEntitiesInCells(Rect rect) {
            return GetAllEntitiesInCells(GetCellsInRect(rect));
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context) {
            foreach (var entity in Entities) {
                PlaceEntityInMatrix(entity);

                // TODO workaround
                entity.Container = this;

                entity.Box.OnChangedEventHandler += rect => PlaceEntityInMatrix(entity);
            }
        }

        private void AllocateMatrix() {
            for (var x = 0; x < _matrixWidth; ++x)
            for (var y = 0; y < _matrixHeight; ++y)
                if (_matrixGrid[x][y] == null)
                    _matrixGrid[x][y] = new List<Entity>();
        }

        private void UpdateActiveCells() {
            ActiveGridCells = GetCellsInRect(ActiveRect);
        }

        private void DrawDebug(SpriteBatch batch, float alpha) {
            if (DrawDebugRects) {
                foreach (var cell in ActiveGridCells) {
                    var rect = GetRectangleOfCell(cell);

                    batch.DrawRectangle(rect.Position.ToVector2(), new Size2(rect.Size.Width, rect.Size.Height),
                        new Color(255, 255, 255, alpha));
                }

                batch.DrawRectangle(ActiveRect.Position.ToVector2(),
                    new Size2(ActiveRect.Size.Width, ActiveRect.Size.Height), new Color(128, 128, 128, alpha));
            }
        }

        private void PlaceEntityInMatrix(Entity entity) {
            foreach (var cell in entity.GridCells) _matrixGrid[cell.X][cell.Y].Remove(entity);

            entity.GridCells.Clear();

            foreach (var cell in GetCellsInRect(entity.Box)) {
                _matrixGrid[cell.X][cell.Y].Add(entity);
                entity.GridCells.Add(cell);
            }
        }

        private Rect GetRectangleOfCell(GridCell cell) {
            return new Rect(cell.X * Constants.MatrixCellSize,
                cell.Y * Constants.MatrixCellSize, Constants.MatrixCellSize, Constants.MatrixCellSize);
        }

        private IEnumerable<GridCell> GetCellsInRect(Rect rect) {
            var cells = new List<GridCell>();

            bool RectContainsCell(GridCell cell) {
                var (x, y) = cell;

                if (x < 0 || y < 0 || x >= MatrixWidth || y >= MatrixHeight) return false;

                return rect.Overlaps(GetRectangleOfCell(cell));
            }

            if (MatrixRect.Overlaps(rect)) {
                var x = Math.Max(0, rect.Position.X.Round() / Constants.MatrixCellSize);
                var y = Math.Max(0, rect.Position.Y.Round() / Constants.MatrixCellSize);

                var cell = new GridCell(x, y);
                if (RectContainsCell(cell)) {
                    cells.Add(cell);
                    var firstXCell = cell.X;

                    while (true) {
                        var nextCellX = new GridCell(x + 1, y);

                        if (RectContainsCell(nextCellX)) {
                            ++x;
                            cells.Add(nextCellX);
                            continue;
                        }

                        x = firstXCell;

                        var nextCellY = new GridCell(x, y + 1);

                        if (RectContainsCell(nextCellY)) {
                            ++y;
                            cells.Add(nextCellY);
                        }
                        else {
                            break;
                        }
                    }
                }
            }

            return cells;
        }
    }
}