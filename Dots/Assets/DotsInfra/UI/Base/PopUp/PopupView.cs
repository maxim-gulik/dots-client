using System;
using Dots.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace Dots.Infra.UI
{
    public interface IPopUpView : IUIActor
    {
        event Action CloseClick;

        Transform ContentTransform { get; }
    }

    public abstract class PopupView : BaseUIActor, IPopUpView
    {
        public event Action CloseClick;

        [SerializeField] private Button[] _closeButtons;
        [SerializeField] private Transform _content;

        public Transform ContentTransform => _content;

        protected override void Awake()
        {
            base.Awake();

            foreach (var button in _closeButtons)
                button.onClick.AddListener(() => CloseClick?.Invoke());
        }

        public override void SetParent(Transform parent, bool worldPositionStays)
        {
            base.SetParent(parent, worldPositionStays);

            Transform.localPosition = Vector3.zero;
            TransformRect.StretchToParentSize();
        }
    }
}