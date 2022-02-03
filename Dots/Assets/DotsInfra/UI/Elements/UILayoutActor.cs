using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dots.Infra.UI
{
    public interface IUILayoutActor : IUIActor
    {
        public Transform BackgroundLayer { get; }
        public Transform MainLayer { get; }
        public Transform ForegroundLayer { get; }
        public Transform ModalsLayer { get; }

        void AddUIActor(IUIActor actor, UILayoutLayerType type);
        IUIActor TryGetUIActor<T>(UILayoutLayerType type) where T : IUIActor;
    }

    public class UILayoutActor : BaseUIActor, IUILayoutActor
    {
        [SerializeField] private Transform _backgroundLayer;
        [SerializeField] private Transform _mainLayer;
        [SerializeField] private Transform _foregroundLayer;
        [SerializeField] private Transform _modalsLayer;

        public Transform BackgroundLayer => _backgroundLayer;
        public Transform MainLayer => _mainLayer;
        public Transform ForegroundLayer => _foregroundLayer;
        public Transform ModalsLayer => _modalsLayer;

        private Dictionary<UILayoutLayerType, Transform> _layerActorsMap = new Dictionary<UILayoutLayerType, Transform>();

        protected override void Awake()
        {
            base.Awake();

            _layerActorsMap.Add(UILayoutLayerType.Background, _backgroundLayer);
            _layerActorsMap.Add(UILayoutLayerType.Main, _mainLayer);
            _layerActorsMap.Add(UILayoutLayerType.Foreground, _foregroundLayer);
            _layerActorsMap.Add(UILayoutLayerType.Modals, _modalsLayer);
        }

        public void AddUIActor(IUIActor actor, UILayoutLayerType type)
        {
            actor.SetParent(_layerActorsMap[type], worldPositionsStays: false);
        }

        public IUIActor TryGetUIActor<T>(UILayoutLayerType type) where T : IUIActor
        {
            return _layerActorsMap[type].GetComponentsInChildren<BaseUIActor>(includeInactive: true).OfType<T>().FirstOrDefault();
        }
    }

    public enum UILayoutLayerType
    {
        Background,
        Main,
        Foreground,
        Modals
    }
}