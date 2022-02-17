using System;

namespace Dots.Extras
{
    public interface IFramePulse
    {
        event Action<float> TickEvent;
    }
}