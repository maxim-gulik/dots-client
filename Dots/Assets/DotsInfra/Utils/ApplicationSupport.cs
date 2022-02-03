using System;
using UnityEngine;

namespace Dots.Infra.Utils
{
    public class ApplicationSupport : MonoBehaviour, IApplicationSupport, IApplicationObserver, IFramePulse, ICoroutineRunner
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
    }
}
