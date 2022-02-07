using System;
using System.Threading;

namespace Dots.Infra.AC
{
    public interface IController : IDisposable
    {
        CancellationToken DisposeToken { get; }
    }
}