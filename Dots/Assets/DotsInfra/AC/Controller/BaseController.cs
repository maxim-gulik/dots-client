using System;
using System.Threading;
using Dots.Extras;

namespace Dots.Infra.AC
{
    public abstract class BaseController : IController, IDisposableOwner
    {
        private readonly CancellationTokenSource _disposeTokeSource = new CancellationTokenSource();
        private readonly DisposablesContainer _disposablesContainer = new DisposablesContainer();

        public CancellationToken DisposeToken => _disposeTokeSource.Token;
        public bool IsControllerDead => DisposeToken.IsCancellationRequested;

        public void Dispose()
        {
            OnDispose();

            _disposeTokeSource.Cancel();
            _disposablesContainer.Dispose();
        }

        protected abstract void OnDispose();

        public void AddOwnership(IDisposable disposable)
        {
            _disposablesContainer.Add(disposable);
        }

        public void RemoveOwnership(IDisposable disposable, bool dispose = false)
        {
            _disposablesContainer.Remove(disposable);

            if (dispose)
                disposable.Dispose();
        }
    }
}