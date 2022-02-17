using System.Collections.Generic;
using System.Linq;
using Dots.AC;
using Dots.Extras;
using Dots.Pool;
using UnityEngine;

namespace DotsGame.Common
{
    /// <summary>
    // Independent system to draw a composite line on the screen by a set of point
    /// </summary>
    public class LinePainterController : BaseController
    {
        private const string LinesObjectId = "DotsLine";

        private readonly IObjectsPool _objectsPool;
        private readonly IFramePulse _framePulse;
        private readonly IApplicationSupport _applicationSupport;

        public readonly List<LineRenderer> _lines = new List<LineRenderer>();
        public readonly Stack<Vector2> _points = new Stack<Vector2>();

        private Color _lineColor;

        public LinePainterController(
            IFramePulse framePulse,
            IApplicationSupport applicationSupport,
            IObjectsPool objectsPool)
        {
            _framePulse = framePulse;
            _applicationSupport = applicationSupport;
            _objectsPool = objectsPool;

            // provide the ability to listen game ticks
            _framePulse.TickEvent += OnTick;
        }

        protected override void OnDispose()
        {
            _framePulse.TickEvent -= OnTick;

            while (_points.Count > 0)
                PopLastLinePoint();
        }

        private void OnTick(float _)
        {
            if(_lines.Count == 0)
                return;

            Vector2 pos = _applicationSupport.GetScreenPointerWorldPosition();
            _lines.Last().SetPosition(1, pos);
        }

        public void SetLineColor(Color color) => _lineColor = color;

        public void PushLinePoint(Vector2 point)
        {
            _points.Push(point);

            var line = _objectsPool.Get<LineRenderer>(LinesObjectId);
            line.startColor = line.endColor = _lineColor;

            //connect the start of new line and the end of previous one to the new point 
            line.SetPosition(0, point);
            if (_lines.Count > 0)
                _lines.Last().SetPosition(1, point);

            _lines.Add(line);
        }

        public void PopLastLinePoint()
        {
            _points.Pop();

            var line = _lines.Last();
            _objectsPool.Return(line);
            _lines.Remove(line);
        }
    }
}