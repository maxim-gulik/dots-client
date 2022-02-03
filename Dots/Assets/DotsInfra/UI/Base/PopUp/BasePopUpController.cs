using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dots.Extras;
using Dots.Infra.AC;
using Dots.Infra.AC.Utils;
using UnityEngine;

namespace Dots.Infra.UI
{
    public interface IPopupController : IController
    {
        event Action Opening;
        event Action Opened;
        event Action Closing;
        event Action Closed;

        public UniTask ActivatePopUpAsync(bool silent, CancellationToken token);
        public UniTask DeactivatePopUpAsync(bool silent, CancellationToken token);
    }

    public interface IPopupController<in TData, TResult> : IPopupController
        where TData : class
    {
        UniTask<TResult> OpenPopUpAsync(TData data, Transform placeholder, CancellationToken token);
        UniTask ClosePopUpAsync(CancellationToken token, TResult result = default);
    }

    public abstract class BasePopUpController<TPopupView, TData, TResult> : BaseController, IPopupController<TData, TResult>
        where TPopupView : class, IPopUpView
        where TData : class
    {
        public event Action Opening;
        public event Action Opened;
        public event Action Closing;
        public event Action Closed;

        private readonly LinkedList<IPopUpLifecycleAction> _actions = new LinkedList<IPopUpLifecycleAction>();
        
        private TData _popUpData;
        private TPopupView _popUpView;
        private UniTaskCompletionSource<TResult> _resultTaskSource;

        private bool IsOpened => _popUpView != null;

        protected abstract UniTask<TPopupView> CreatePopupViewAsync(CancellationToken token);

        protected virtual UniTask OnOpeningAsync(TPopupView view, TData data, CancellationToken token) => InvokeActionsAsync(e => e.OnOpeningAsync(view, token));
        protected virtual UniTask OnOpenedAsync(TPopupView view, TData data, CancellationToken token) => InvokeActionsAsync(e => e.OnOpenedAsync(view, token));
        protected virtual UniTask OnClosingAsync(TPopupView view, TData data, CancellationToken token) => InvokeActionsAsync(e => e.OnClosingAsync(view, token));
        protected virtual UniTask OnClosedAsync(TData data, CancellationToken token) => InvokeActionsAsync(e => e.OnClosedAsync(token));

        public async UniTask<TResult> OpenPopUpAsync(TData data, Transform placeholder, CancellationToken token)
        {
            _popUpData = data;

            var cancelToken = CancellationTokenSource.CreateLinkedTokenSource(token, DisposeToken).Token;

            await new Flow(cancelToken, $"OpenPopUpFlow_{GetType().Name}")
                .WithTask(() => PrepareViewAsync(placeholder, cancelToken))
                .WithVoid(() => Opening?.Invoke())
                .WithTask(() => OnOpeningAsync(_popUpView, _popUpData, cancelToken))
                .WithTask(() => ActivatePopUpAsync(silent:true, token))
                .WithTask(() => OnOpenedAsync(_popUpView, _popUpData, cancelToken))
                .WithVoid(() => Opened?.Invoke())
                .RunAsync();

            _resultTaskSource = new UniTaskCompletionSource<TResult>();

            return await WaitResultAsync(cancelToken);
        }

        public UniTask ClosePopUpAsync(CancellationToken token, TResult result)
        {
            if(!IsOpened)
                return UniTask.CompletedTask;

            var cancelToken = CancellationTokenSource.CreateLinkedTokenSource(token, DisposeToken).Token;

            return new Flow(cancelToken, $"ClosePopUpFlow_{GetType().Name}")
                .WithVoid(() => Closing?.Invoke())
                .WithTask(() => OnClosingAsync(_popUpView, _popUpData, cancelToken))
                .WithTask(() => DeactivatePopUpAsync(silent: true, cancelToken))
                .WithVoid(DisposeView)
                .WithTask(() => OnClosedAsync(_popUpData, cancelToken))
                .WithVoid(() => Closed?.Invoke())
                .WithVoid(() => CompleteWithResult(result))
                .RunAsync();
        }

        public async UniTask ActivatePopUpAsync(bool silent, CancellationToken token)
        {
            if (!IsOpened)
                throw new InvalidOperationException($"Impossible to activate not opened popup. PopUp: {GetType().Name}");

            if (_popUpView.IsActive)
                return;

            await InvokeActionsAsync(a => a.OnActivatingAsync(silent, _popUpView, token));
            if (token.IsCancellationRequested)
                return;

            _popUpView.IsActive = true;

            await InvokeActionsAsync(a => a.OnActivatedAsync(silent, _popUpView, token));
        }

        public async UniTask DeactivatePopUpAsync(bool silent, CancellationToken token)
        {
            if (!IsOpened)
                throw new InvalidOperationException($"Impossible to deactivate not opened popup. PopUp: {GetType().Name}");

            if (!_popUpView.IsActive)
                return;

            await InvokeActionsAsync(a => a.OnDeactivatingAsync(silent, _popUpView, token));
            if(token.IsCancellationRequested)
                return;

            _popUpView.IsActive = false;
            
            await InvokeActionsAsync(a => a.OnDeactivatedAsync(silent, _popUpView, token));
        }

        protected void AddLifecycleActions(params IPopUpLifecycleAction[] actions)
        {
            foreach (var a in actions)
                _actions.AddLast(a);
        }

        private UniTask<TResult> WaitResultAsync(CancellationToken token)
        {
            _resultTaskSource = new UniTaskCompletionSource<TResult>();
            return _resultTaskSource.Task.WithFastCancellationSafe(token);
        }

        private void CompleteWithResult(TResult result)
        {
            if (!_resultTaskSource.Task.Status.IsCompleted())
            {
                _resultTaskSource.TrySetResult(result);
            }
        }

        protected override void OnDispose()
        {
            _actions.ForEach(a => a.OnDispose());

            if (_popUpView != null)
                DisposeView();
        }

        private async UniTask PrepareViewAsync(Transform placeholder, CancellationToken token)
        {
            _popUpView = await CreatePopupViewAsync(token);
            _popUpView.IsActive = false;
            _popUpView.SetParent(placeholder, worldPositionsStays: false);
        }

        private void DisposeView()
        {
            _popUpView.Destroy();
            _popUpView = null;
        }

        private UniTask InvokeActionsAsync(Func<IPopUpLifecycleAction, UniTask> action) => UniTask.WhenAll(UniTaskUtils.Select(_actions, action));
    }

    public abstract class BasePopUpController<TPopupView> : BasePopUpController<TPopupView, None, None>
        where TPopupView : class, IPopUpView
    {
        public UniTask ClosePopUpAsync(CancellationToken token)
        {
            return ClosePopUpAsync(token, None.Value);
        }
    }

    public abstract class BasePopUpController<TPopupView, TData> : BasePopUpController<TPopupView, TData, None>
        where TPopupView : class, IPopUpView
        where TData : class
    {
        public UniTask ClosePopUpAsync(CancellationToken token)
        {
            return ClosePopUpAsync(token, None.Value);
        }
    }
}