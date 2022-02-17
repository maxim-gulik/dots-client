using System;
using System.Threading;

namespace Dots.AC
{
    public interface IController : IDisposable
    {
        CancellationToken DisposeToken { get; }
    }
}