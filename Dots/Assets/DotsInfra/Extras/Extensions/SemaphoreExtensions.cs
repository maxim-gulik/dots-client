using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dots.Extras
{
    public static class SemaphoreExtensions
    {
        private class DisposableSource : IDisposable
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

        public static async UniTask<IDisposable> WaitAsyncModal(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new DisposableSource(() => semaphore.Release());
        }
    }
}