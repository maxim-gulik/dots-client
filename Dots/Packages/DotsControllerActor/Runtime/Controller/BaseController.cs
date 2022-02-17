using System;
using System.Threading;
using Dots.Extras;

namespace Dots.AC
{
    /// <summary>
    /// Base class for all controllers (classes responsible for the business logic of a game)
    /// Disposable and can be as an owner for a set of disposable instances
    /// </summary>
    public abstract class BaseController : IController, IDisposableOwner
    {
        private readonly CancellationTokenSource _disposeTokeSource = new CancellationTokenSource();
        private readonly DisposablesContainer _disposablesContainer = new DisposablesContainer();

        public CancellationToken DisposeToken => _disposeTokeSource.Token;

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