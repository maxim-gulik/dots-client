using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dots.Extras;
using Dots.AC;
using DotsGame.EndlessMode;
using UnityEngine;

namespace DotsGame.Common
{
    //Independent system to layout and controller Dots on the game field
    public class DotsFieldController : BaseController
    {
        private readonly IGetActorCommand _getActorCommand;
        private readonly IInstantiateActorCommand _instantiateActorCommand;

        private readonly GridActor _grid;
        private readonly IReadOnlyList<DotActor> _dots;
        private readonly Dictionary<int, List<DotActor>> _dotsColumnMap = new Dictionary<int, List<DotActor>>();
        private readonly Dictionary<int, List<DotActor>> _freeDotsColumnMap = new Dictionary<int, List<DotActor>>();

        private readonly GameTheme _theme;

        public DotsFieldController(
            Parameters parameters,
            IGetActorCommand getActorCommand,
            IInstantiateActorCommand instantiateActorCommand)
        {
            _getActorCommand = getActorCommand;
            _instantiateActorCommand = instantiateActorCommand;

            _theme = parameters.Theme;

            _grid = CreateGrid(parameters.GridSettings);
            _dots = CreateDots();
        }

        protected override void OnDispose()
        {
            foreach (var dot in _dots)
                dot.Destroy();

            _grid.Destroy();
        }

        public IReadOnlyList<DotActor> GetDots() => _dots;

        public List<DotActor> FindDots(Func<DotActor, bool> filter) => _dots.Where(filter).ToList();

        public UniTask AppearDots()
        {
            var tasks = new List<UniTask>(_dots.Count);

            foreach (var dot in _dots) 
                tasks.Add(DropDotFromRoof(dot));

            return UniTask.WhenAll(tasks);
        }

        public UniTask FreeDots(IReadOnlyList<DotActor> dots)
        {
            var tasks = new List<UniTask>();
            foreach (var dot in dots)
            {
                //add the dot to a relevant column to the the pool
                _freeDotsColumnMap.GetOrNew(dot.GridCoords.x).Add(dot);

                tasks.Add(dot.PlayHideAnimation());
            }

            return UniTask.WhenAll(tasks);
        }

        public UniTask DropHangingDots()
        {
            var tasks = new List<UniTask>();

            //drop dots column by column
            foreach (var column in _freeDotsColumnMap.Keys)
            {
                var freeDots = _freeDotsColumnMap[column];
                var activeDots = _dotsColumnMap[column].Where(d => !freeDots.Contains(d));

                foreach (var activeDot in activeDots)
                {
                    //detect free space under the dot
                    var freeUnder = freeDots.Count(freeDot => activeDot.GridCoords.y > freeDot.GridCoords.y);
                    if (freeUnder == 0)
                        continue;

                    //drop the dot down
                    var row = activeDot.GridCoords.y - freeUnder;
                    activeDot.GridCoords = new Vector2Int(column, row);
                    var dropPos = _grid.GetPosByCellCoords(activeDot.GridCoords);
                    tasks.Add(activeDot.PlayDropAnimation(dropPos));
                }
            }

            return UniTask.WhenAll(tasks);
        }

        public UniTask FillFreeSpaceByDots()
        {
            var tasks = new List<UniTask>();
            foreach (var column in _freeDotsColumnMap.Keys)
            {
                var freeDots = _freeDotsColumnMap[column];
                for (var i = 0; i < freeDots.Count; i++)
                {
                    var dot = freeDots[i];

                    //set up dot
                    dot.Color = _theme.GetRandomDotColor();
                    var toDropRow = _grid.Rows - freeDots.Count + i;
                    dot.GridCoords = new Vector2Int(column, toDropRow);

                    tasks.Add(DropDotFromRoof(dot));
                }
            }

            //free dots cache has to be cleared because it contained exect amount of dots that were needed to fill free space above
            _freeDotsColumnMap.Clear();

            return UniTask.WhenAll(tasks);
        }

        public UniTask DropDotFromRoof(DotActor dot)
        {
            const float roofY = 7;

            var to = _grid.GetPosByCellCoords(dot.GridCoords);
            var from = new Vector2(to.x, roofY);

            return dot.PlayDropAnimation(from, to);
        }

        private GridActor CreateGrid(GridSettings gridSetting)
        {
            var grid = _getActorCommand.Execute<GridActor>();
            grid.Construct(gridSetting);
            return grid;
        }

        private IReadOnlyList<DotActor> CreateDots()
        {
            var dots = new List<DotActor>(_grid.Rows * _grid.Columns);

            for (byte c = 0; c < _grid.Columns; c++)
            for (byte r = 0; r < _grid.Rows; r++)
            {
                //instantiate only once per game than used from the cache
                var dot = _instantiateActorCommand.Execute<DotActor>(_grid, worldPositionStays: false);
                dot.Construct(gridCoords: new Vector2Int(c, r), _theme.GetRandomDotColor());
                
                _dotsColumnMap.GetOrNew(c).Add(dot);
                dots.Add(dot);
            }

            return dots;
        }

        public class Parameters
        {
            public readonly GameTheme Theme;
            public readonly GridSettings GridSettings;

            public Parameters(GameTheme theme, GridSettings gridSettings)
            {
                Theme = theme;
                GridSettings = gridSettings;
            }
        }
    }
}