using Dots.Infra.AC;
using UnityEngine;

namespace Dots.Infra.UI
{
    public interface IUIActor : IActor
    {
        RectTransform TransformRect { get; }
    }
}