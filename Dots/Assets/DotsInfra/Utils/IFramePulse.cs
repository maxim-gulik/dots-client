using System;

namespace Dots.Infra.Utils
{
    public interface IFramePulse
    {
        event Action<float> TickEvent;
    }
}