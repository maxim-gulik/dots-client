using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DotsGame.Common
{
    /// <summary>
    /// The contract for the set of filters that can be used for dots selection
    /// </summary>
    public interface IPickedDotFilter
    {
        bool Match(DotActor pickedDot, IReadOnlyList<DotActor> selectedDots);
    }

    public class PickedNonLastFilter : IPickedDotFilter
    {
        public bool Match(DotActor pickedDot, IReadOnlyList<DotActor> selectedDots) => selectedDots.Last() != pickedDot;
    }

    public class PickedAndFirstWithSameColorFilter : IPickedDotFilter
    {
        public bool Match(DotActor pickedDot, IReadOnlyList<DotActor> selectedDots) => selectedDots.First().Color == pickedDot.Color;
    }

    public class PickedAndLastNearbyFilter : IPickedDotFilter
    {
        public bool Match(DotActor pickedDot, IReadOnlyList<DotActor> selectedDots)
        {
            var rowDiff = Mathf.Abs(pickedDot.GridCoords.y - selectedDots.Last().GridCoords.y);
            var columnDiff = Mathf.Abs(pickedDot.GridCoords.x - selectedDots.Last().GridCoords.x);
            var nonNearby = rowDiff > 1 || columnDiff > 1;
            return !nonNearby;
        }
    }

    public class PickedAndLastNonDiagonalFilter : IPickedDotFilter
    {
        public bool Match(DotActor pickedDot, IReadOnlyList<DotActor> selectedDots)
        {
            var rowDiff = Mathf.Abs(pickedDot.GridCoords.y - selectedDots.Last().GridCoords.y);
            var columnDiff = Mathf.Abs(pickedDot.GridCoords.x - selectedDots.Last().GridCoords.x);
            var diagonal = rowDiff == columnDiff && rowDiff > 0;
            return !diagonal;
        }
    }

    public class PickedClosedLoopFilter : IPickedDotFilter
    {
        public bool Match(DotActor pickedDot, IReadOnlyList<DotActor> selectedDots)
        {
            return !selectedDots.Contains(pickedDot) || selectedDots.Count == selectedDots.Distinct().Count();
        }
    }

    public class PickedOneOfLastFilter : IPickedDotFilter
    {
        private readonly int _range;

        public PickedOneOfLastFilter(int range)
        {
            _range = range;
        }

        public bool Match(DotActor pickedDot, IReadOnlyList<DotActor> selectedDots)
        {
            var i = Mathf.Max(0, selectedDots.Count - _range);
            for (; i < selectedDots.Count; i++)
                if (selectedDots[i] == pickedDot)
                    return true;

            return false;
        }
    }
}