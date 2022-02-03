using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dots.Extras;
using Dots.Infra.AC;
using Dots.Infra.AC.Utils;

namespace Dots.Infra.UI
{
    public enum PopUpType
    {
        Single,
        Overlap
    }

    public interface IPopUpsStack : IDisposable
    {
        UniTask<None> OpenPopupAsync<TController>(
            PopUpType type,
            CancellationToken token)
            where TController : IPopupController<None, None>;

        UniTask<None> OpenPopupAsync<TController, TData>(
            TData data,
            PopUpType type,
            CancellationToken token)
            where TController : IPopupController<TData, None>
            where TData : class;

        UniTask<TResult> OpenPopupAsync<TController, TResult>(
            PopUpType type,
            CancellationToken token)
            where TController : IPopupController<None, TResult>;

        UniTask<TResult> OpenPopupAsync<TController, TData, TResult>(
            TData data,
            PopUpType type,
            CancellationToken token = default)
            where TController : IPopupController<TData, TResult>
            where TData : class;
    }

    public class PopUpsStack : IPopUpsStack
    {
        private readonly IUILayoutActor _uiLayout;
        private readonly ICreateControllerCommand _createControllerCommand;

        private readonly LinkedList<IPopupController> _popUps = new LinkedList<IPopupController>();

        public PopUpsStack(
            IUILayoutActor uiLayout,
            ICreateControllerCommand createControllerCommand)
        {
            _uiLayout = uiLayout;
            _createControllerCommand = createControllerCommand;
        }

        public UniTask<None> OpenPopupAsync<TController>(
            PopUpType type,
            CancellationToken token)
            where TController : IPopupController<None, None>
        {
            return OpenPopupAsync<TController, None, None>(None.Value, type, token);
        }

        public UniTask<None> OpenPopupAsync<TController, TData>(
            TData data,
            PopUpType type,
            CancellationToken token)
            where TController : IPopupController<TData, None>
            where TData : class
        {
            return OpenPopupAsync<TController, TData, None>(data, type, token);
        }

        public UniTask<TResult> OpenPopupAsync<TController, TResult>(
            PopUpType type,
            CancellationToken token)
            where TController : IPopupController<None, TResult>
        {
            return OpenPopupAsync<TController, None, TResult>(None.Value, type, token);
        }

        public async UniTask<TResult> OpenPopupAsync<TController, TData, TResult>(
            TData data,
            PopUpType type,
            CancellationToken token = default)
            where TController : IPopupController<TData, TResult>
            where TData : class
        {
            var controller = _createControllerCommand.Execute<TController>();

            if (type == PopUpType.Single)
                await HideAllPopUpsAsync(token);

            controller.Closed += () =>
            {
                _popUps.Remove(controller);
                controller.Dispose();

                if (type == PopUpType.Single)
                    TryShowLastPopUpAsync(token).Forget();
            };

            _popUps.AddLast(controller);

            return await controller.OpenPopUpAsync(data, _uiLayout.ModalsLayer, token);
        }

        private UniTask HideAllPopUpsAsync(CancellationToken token)
        {
            if(_popUps.Count == 0)
                return UniTask.CompletedTask;

            var tasks = UniTaskUtils.Select(_popUps, popUp => popUp.DeactivatePopUpAsync(silent: false, token));
            return UniTask.WhenAll(tasks);
        }

        private UniTask TryShowLastPopUpAsync(CancellationToken token)
        {
            return _popUps.Count == 0
                ? UniTask.CompletedTask
                : _popUps.Last().ActivatePopUpAsync(silent: false, token);
        }

        public void Dispose()
        {
            foreach (var popup in _popUps)
                popup.Dispose();

            _popUps.Clear();
        }
    }
}