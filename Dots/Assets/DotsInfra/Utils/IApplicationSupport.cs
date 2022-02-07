using UnityEngine;

namespace Dots.Infra.Utils
{
    public interface IApplicationSupport
    {
        string ProductName { get; }
        string Version { get; }
        string PersistentDataPath { get; }
        RuntimePlatform Platform { get; }

        Vector3 GetScreenPointerWorldPosition();
        void OpenUrl(string url);
        void Quit();
    }
}