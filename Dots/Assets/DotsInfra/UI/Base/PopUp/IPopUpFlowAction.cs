using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dots.Infra.UI
{
    public abstract class BasePopULifecycleAction : IPopUpLifecycleAction
    {
        public abstract int Id { get; }

        public virtual UniTask OnOpeningAsync(IPopUpView view, CancellationToken token) => UniTask.CompletedTask;
        public virtual UniTask OnOpenedAsync(IPopUpView view, CancellationToken token) => UniTask.CompletedTask;
        public virtual UniTask OnActivatingAsync(bool silent, IPopUpView view, CancellationToken token) => UniTask.CompletedTask;
        public virtual UniTask OnActivatedAsync(bool silent, IPopUpView view, CancellationToken token) => UniTask.CompletedTask;
        public virtual UniTask OnDeactivatingAsync(bool silent, IPopUpView view, CancellationToken token) => UniTask.CompletedTask;
        public virtual UniTask OnDeactivatedAsync(bool silent, IPopUpView view, CancellationToken token) => UniTask.CompletedTask;
        public virtual UniTask OnClosingAsync(IPopUpView view, CancellationToken token) => UniTask.CompletedTask;
        public virtual UniTask OnClosedAsync(CancellationToken token) => UniTask.CompletedTask;

        public virtual void OnDispose()
        {
        }
    }
}