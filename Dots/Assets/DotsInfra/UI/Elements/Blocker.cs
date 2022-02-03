using System;
using Dots.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace Dots.Infra.UI
{
    public interface IBlocker
    {
        void SetActive(bool active);
        IDisposable ExecuteModal();
    }
    
    [RequireComponent(typeof(Image))]
    public class Blocker : MonoBehaviour, IBlocker
    {
        private Image _image;
        private int _counter;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.enabled = false;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public IDisposable ExecuteModal()
        {
            if (_counter == 0)
            {
                _image.enabled = true;
            }
            
            _counter++;
            return new DisposableSource(Hide);

            void Hide()
            {
                _counter--;

                if (_counter == 0)
                {
                    _image.enabled = false;
                }
            }
        }
    }
}