using System;

namespace Dots.Extras
{
    public class DisposableSource : IDisposable
    {
        private readonly Action _disposeAction;

        public DisposableSource(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            _disposeAction();
        }
    }
}