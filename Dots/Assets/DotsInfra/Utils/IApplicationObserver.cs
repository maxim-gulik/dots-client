using System;

namespace Dots.Infra.Utils
{
    public interface IApplicationObserver
    {
        event Action<bool> PauseEvent;
        event Action QuitEvent;
    }
}