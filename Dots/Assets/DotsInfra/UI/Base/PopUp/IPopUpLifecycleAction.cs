using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dots.Infra.UI
{
    public interface IPopUpLifecycleAction
    {
        int Id { get; }

        UniTask OnOpeningAsync(IPopUpView view, CancellationToken token);
        UniTask OnOpenedAsync(IPopUpView view, CancellationToken token);
        UniTask OnActivatingAsync(bool silent, IPopUpView view, CancellationToken token);
        UniTask OnActivatedAsync(bool silent, IPopUpView view, CancellationToken token);
        UniTask OnDeactivatingAsync(bool silent, IPopUpView view, CancellationToken token);
        UniTask OnDeactivatedAsync(bool silent, IPopUpView view, CancellationToken token);
        UniTask OnClosingAsync(IPopUpView view, CancellationToken token);
        UniTask OnClosedAsync(CancellationToken token);
        void OnDispose();
    }
}