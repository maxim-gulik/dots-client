using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dots.AC;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DotsGame.Common
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DotActor :
        BaseActor,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerUpHandler
    {
        public event Action<DotActor> PickedEvent;
        public event Action<DotActor> EnteredEvent;
        public event Action PointerUpEvent;

        [SerializeField] private SpriteRenderer _backSprite;
        [SerializeField] private SpriteRenderer _frontSprite;

        private SpriteRenderer[] _sprites;

        private float _defaultAlpha;
        private Vector3 _defaultScale;
        private Color _color;

        public void Construct(Vector2Int gridCoords, Color color)
        {
            _sprites = new[]
            {
                _backSprite,
                _frontSprite
            };
            _defaultAlpha = _frontSprite.color.a;
            _defaultScale = _frontSprite.transform.localScale;
            
            GridCoords = gridCoords;
            Color = color;
        }

        public Vector2Int GridCoords { get; set; }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                foreach (var s in _sprites)
                    s.color = _color;
            }
        }

        public void OnPointerDown(PointerEventData eventData) => PickedEvent?.Invoke(this);

        public void OnPointerEnter(PointerEventData eventData) => EnteredEvent?.Invoke(this);

        public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke();

        public UniTask PlayDropAnimation(Vector2 to) => PlayDropAnimation(Transform.localPosition, to);

        public async UniTask PlayHideAnimation()
        {
            const float hideScale = 0.4f;
            const float durationS = 0.1f;

            await Transform.DOScale(hideScale, durationS).ToUniTask();
            transform.localScale = _defaultScale;
        }

        public UniTask PlayDropAnimation(Vector2 from, Vector2 to)
        {
            const float durationS = 0.4f;
            const float rowDelay = 0.06f;

            Transform.localPosition = from;
            return Transform.DOMove(to, durationS)
                .SetEase(Ease.OutBounce)
                .SetDelay(rowDelay * GridCoords.y)
                .ToUniTask();
        }

        public async UniTask PlayTouchAnimation()
        {
            const float durationS = 0.6f;
            const float scale = 2f;
            const float frontAlpha = 0.8f;

            _frontSprite.SetSColorA(frontAlpha);
            await UniTask.WhenAll(
                _frontSprite.transform.DOScale(new Vector2(scale, scale), durationS).ToUniTask(),
                _frontSprite.DOFade(0f, durationS).ToUniTask());

            await _frontSprite.transform.DOScale(_defaultScale, durationS).ToUniTask();
            _frontSprite.SetSColorA(_defaultAlpha);
        }
    }
}