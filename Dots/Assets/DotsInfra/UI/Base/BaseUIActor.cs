using Dots.Infra.AC;
using UnityEngine;

namespace Dots.Infra.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseUIActor : BaseActor, IUIActor
    {
        public RectTransform TransformRect { get; private set; }

        protected virtual void Awake()
        {
            TransformRect = GetComponent<RectTransform>();
        }
    }
}