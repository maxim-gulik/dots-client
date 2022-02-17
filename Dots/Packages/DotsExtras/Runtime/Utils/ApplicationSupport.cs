using System;
using UnityEngine;

namespace Dots.Extras
{
    public class ApplicationSupport : MonoBehaviour, IApplicationSupport, IFramePulse
    {
        public event Action<bool> PauseEvent;
        public event Action QuitEvent;
        public event Action<float> TickEvent; //delta in seconds

        public string ProductName => Application.productName;
        public string Version => Application.version;
        public string PersistentDataPath => Application.persistentDataPath;
        public RuntimePlatform Platform => Application.platform;

        private void OnApplicationPause(bool pause)
        {
            PauseEvent?.Invoke(pause);
        }

        private void OnApplicationQuit()
        {
            QuitEvent?.Invoke();
        }

        private void Update()
        {
            TickEvent?.Invoke(Time.deltaTime);
        }

        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public Vector3 GetScreenPointerWorldPosition()
        {
            Vector3 poinerPos;
            if (Input.touchSupported)
            {
                poinerPos = Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;
            }
            else
            {
                poinerPos = Input.mousePosition;
            }
            return Camera.main.ScreenToWorldPoint(poinerPos);
        }
    }
}
