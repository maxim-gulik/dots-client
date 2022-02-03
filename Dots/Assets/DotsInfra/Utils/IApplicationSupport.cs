using UnityEngine;

namespace Dots.Infra.Utils
{
    public interface IApplicationSupport
    {
        string ProductName { get; }
        string Version { get; }
        string PersistentDataPath { get; }
        RuntimePlatform Platform { get; }

        void OpenUrl(string url);
        void Quit();
    }
}