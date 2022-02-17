using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dots.AC;

namespace DotsGame.Common
{
    /// <summary>
    /// Independent system to detect interaction with dots and select ones which match passed filers
    /// </summary>
    public class DotsSelectionController : BaseControllerWithResult<IReadOnlyList<DotActor>>
    {
        //param1 - isFirst, param2 - selected dot
        public event Action<bool, DotActor> DotSelectedEvent;
        public event Action<DotActor> DotCanceledEvent;

        private readonly IReadOnlyCollection<DotActor> _dots;
        private readonly IReadOnlyCollection<IPickedDotFilter> _allowSelectionFilters;
        private readonly IReadOnlyCollection<IPickedDotFilter> _cancelSelectionFilters;

        private bool _selectionStarted;
        private List<DotActor> _selectedDots;

        public DotsSelectionController(Parameters parameters)
        {
            _dots = parameters.Dots;
            _allowSelectionFilters = parameters.AllowDotSelectFilters;
            _cancelSelectionFilters = parameters.CancelDotSelectionFilters;
        }

        protected override void OnDispose() { }

        public void OnDotPicked(DotActor actor)
        {
            if (!_selectionStarted)
            {
                _selectionStarted = true;

                ProcessPickedDot(actor);
            }
        }

        public void OnPointerEnter(DotActor actor)
        {
            if (_selectionStarted)
                ProcessPickedDot(actor);
        }

        public void OnPointerUp()
        {
            if (_selectionStarted)
            {
                _selectionStarted = false;

                //Complete the controller work and notify a parent system about getting the result
                ReturnControllerResult(_selectedDots);
            }
        }

        protected override UniTask OnStartAsync()
        {
            _selectedDots = new List<DotActor>();

            foreach (var d in _dots)
            {
                d.PickedEvent += OnDotPicked;
                d.EnteredEvent += OnPointerEnter;
                d.PointerUpEvent += OnPointerUp;
            }

            return UniTask.CompletedTask;
        }

        protected override UniTask OnCompleteAsync()
        {
            foreach (var d in _dots)
            {
                d.PickedEvent -= OnDotPicked;
                d.EnteredEvent -= OnDotPicked;
                d.PointerUpEvent -= OnPointerUp;
            }

            return base.OnCompleteAsync();
        }

        private void ProcessPickedDot(DotActor dot)
        {
            if (CheckIfDotSelectionCanceled(dot))
            {
                UnselectDot(dot);
                return;
            }

            var isFirstDot = _selectedDots.Count == 0;
            if (isFirstDot || CheckIfDotSelected(dot))
            {
                SelectDot(isFirstDot, dot);
            }
        }

        public bool CheckIfDotSelected(DotActor dot) => _allowSelectionFilters.All(r => r.Match(dot, _selectedDots));

        public bool CheckIfDotSelectionCanceled(DotActor dot) => _cancelSelectionFilters.All(r => r.Match(dot, _selectedDots));

        private void SelectDot(bool isFirst, DotActor dot)
        {
            _selectedDots.Add(dot);
            DotSelectedEvent?.Invoke(isFirst, dot);
        }

        private void UnselectDot(DotActor dot)
        {
            _selectedDots.Remove(dot);
            DotCanceledEvent?.Invoke(dot);
        }

        public class Parameters
        {
            public readonly IReadOnlyCollection<DotActor> Dots;
            public readonly IReadOnlyCollection<IPickedDotFilter> AllowDotSelectFilters;
            public readonly IReadOnlyCollection<IPickedDotFilter> CancelDotSelectionFilters;

            public Parameters(
                IReadOnlyCollection<DotActor> dots,
                IReadOnlyCollection<IPickedDotFilter> allowDotSelectFilters,
                IReadOnlyCollection<IPickedDotFilter> cancelDotSelectionFilters)
            {
                Dots = dots;
                AllowDotSelectFilters = allowDotSelectFilters;
                CancelDotSelectionFilters = cancelDotSelectionFilters;
            }
        }
    }
}