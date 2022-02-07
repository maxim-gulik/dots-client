using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dots.Infra.AC;
using DotsGame.Common;

namespace DotsGame.EndlessMode
{
    /// <summary>
    /// The root controller for the endless game mod
    /// </summary>
    public class EndlessGameController : BaseController
    {
        private const int MinimumSelectionDotsAmount = 2;

        private readonly IReadOnlyList<IPickedDotFilter> _allowDotSelectionFilters = new List<IPickedDotFilter>
        {
            new PickedNonLastFilter(),
            new PickedAndFirstWithSameColorFilter(),
            new PickedAndLastNearbyFilter(),
            new PickedAndLastNonDiagonalFilter(),
            new PickedClosedLoopFilter()
        };   
        
        private readonly IReadOnlyList<IPickedDotFilter> _cancelDotSelectionFilters = new List<IPickedDotFilter>
        {
            new PickedOneOfLastFilter(range: 2)
        };

        private readonly ICreateControllerCommand _createControllerCommand;
        private readonly IGetActorCommand _getActorCommand;
        private readonly EndlessModeGameSettings _endlessModeSettings;
        private readonly GameTheme _theme;

        private DotsFieldController _fieldController;
        private LinePainterController _linePainterController;

        public EndlessGameController(
            EndlessModeGameSettings endlessModeSettings,
            ICreateControllerCommand createControllerCommand,
            IGetActorCommand getActorCommand)
        {
            _endlessModeSettings = endlessModeSettings;
            _createControllerCommand = createControllerCommand;
            _getActorCommand = getActorCommand;

            _theme = endlessModeSettings.GetRandomTheme();
            _theme.SetMaxDotsColorAmount(endlessModeSettings.MaxDotsColors);
        }

        protected override void OnDispose() { }

        //the main method contains full consequence game flow
        public async UniTask PlayDotsAsync()
        {
            SetupSettings();

            await AppearDots();
            if (DisposeToken.IsCancellationRequested)
                return;

            //game loop
            while (!DisposeToken.IsCancellationRequested)
            {
                var selectionResult = await AwaitDotsSelection();
                if(DisposeToken.IsCancellationRequested)
                    return;

                await ProcessSelectedDots(selectionResult);
            }
        }

        private UniTask AppearDots()
        {
            //[param] are set of custom parameters that are passed into a controller in addition to injected ones
            //[owner] means that a created controller will be attached to an IDisposableOwner instance became responsible for disposing it
            var param = new DotsFieldController.Parameters(_theme, _endlessModeSettings.GridSettings);
            _fieldController = _createControllerCommand.Execute<DotsFieldController, DotsFieldController.Parameters>(param, owner: this);
            return _fieldController.AppearDots();
        }

        private async UniTask<IReadOnlyList<DotActor>> AwaitDotsSelection()
        {
            var dots = _fieldController.GetDots();
            var param = new DotsSelectionController.Parameters(dots, _allowDotSelectionFilters, _cancelDotSelectionFilters);

            var selectionController = _createControllerCommand.Execute<DotsSelectionController, DotsSelectionController.Parameters>(param, owner: this);
            _linePainterController = _createControllerCommand.Execute<LinePainterController>(owner: selectionController);

            selectionController.DotSelectedEvent += OnDotSelected;
            selectionController.DotCanceledEvent += OnDotCanceledEvent;

            //wait until dots will be selected (see DotsSelectionController description for more info)
            var result = await selectionController.GetResultAsync(DisposeToken);
            if (DisposeToken.IsCancellationRequested)
                return result;

            selectionController.DotSelectedEvent -= OnDotSelected;
            selectionController.DotCanceledEvent -= OnDotCanceledEvent;

            //get rid of child controller and dispose it. _linePainterController will be disposed also, because it is a child of _selectionController
            RemoveOwnership(selectionController, dispose: true);

            return result;
        }

        private void OnDotSelected(bool isFirst, DotActor dot)
        {
            if (isFirst)
                _linePainterController.SetLineColor(dot.Color);

            _linePainterController.PushLinePoint(dot.WorldPosition);

            dot.PlayTouchAnimation().Forget();
        }

        private void OnDotCanceledEvent(DotActor _)
        {
            _linePainterController.PopLastLinePoint();
        }

        private async UniTask ProcessSelectedDots(IReadOnlyList<DotActor> dots)
        {
            if (dots.Count < MinimumSelectionDotsAmount)
                return;

            var collectedDots = DetectDotsCanBeCollected(dots);

            var freeDotsTask = _fieldController.FreeDots(collectedDots);

            _fieldController.DropHangingDots().Forget();

            await freeDotsTask;
            if(DisposeToken.IsCancellationRequested)
                return;

            await _fieldController.FillFreeSpaceByDots();
        }

        private IReadOnlyList<DotActor> DetectDotsCanBeCollected(IReadOnlyList<DotActor> selectedDots)
        {
            //in case of looped selection just find all dots with the same type in the field to remove
            var areLooped = selectedDots.Count != selectedDots.Distinct().Count();
            var selectionColor = selectedDots.First().Color;

            return areLooped ?
                _fieldController.FindDots(d => d.Color == selectionColor)
                : selectedDots;
        }

        private void SetupSettings()
        {
            //just get registered instance of CameraActor
            var camera = _getActorCommand.Execute<CameraActor>("MainCamera");
            camera.SetBackgroundColor(_theme.BackgroundColor);
        }
    }
}