using DG.Tweening;
using Dots.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace Dots.Infra.UI
{
    public interface IFaderActor : IUIActor
    {
        void Show();
        void Hide();
    }
    
    [RequireComponent(typeof(Image))]
    public class FaderActor : BaseUIActor, IFaderActor
    {
        [SerializeField, Range(0f, 1f)] private float _fadeAlpha;
        [SerializeField] private float _durationS = 0.3f;
        
        private Image _fade;
        private int _counter;
        private Tweener _fadeTweener;
        
        protected override void Awake()
        {
            base.Awake();

            _fade = GetComponent<Image>();
            _fade.SetAlpha(0);
            _fade.raycastTarget = false;
        }

        public void Show()
        {
            if (_counter == 0)
            {
                _fadeTweener?.Kill();
                
                _fade.SetAlpha(0);
                _fadeTweener = _fade.DOFade(_fadeAlpha, _durationS);
            }
            
            _counter++;
        }

        public void Hide()
        {
            _counter--;

            if (_counter == 0)
            {
                _fadeTweener?.Kill();
                _fadeTweener = _fade.DOFade(0, _durationS);
            }
        }
    }
}