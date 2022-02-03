using System;
using System.Threading;

namespace Dots.Infra.AC
{
    public interface IController : IDisposable
    {
        bool IsControllerDead { get; }
        CancellationToken DisposeToken { get; }
    }
}