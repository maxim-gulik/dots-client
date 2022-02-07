using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dots.Infra.AC
{
    public interface IControllerWithResult<TResult> : IController
    {
        UniTask<TResult> GetResultAsync(CancellationToken token);
    }

    /// <summary>
    /// This controller has an special API allows to wait until the controller will return an result via ReturnControllerResult()
    /// Contains a couple lifecycle methods in different stages of getting result flow
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class BaseControllerWithResult<TResult> : BaseController, IControllerWithResult<TResult>
    {
        private readonly UniTaskCompletionSource<TResult> _taskSource = new UniTaskCompletionSource<TResult>();

        public CancellationToken ExecutionToken { get; private set; }

        protected abstract UniTask OnStartAsync();

        protected virtual UniTask OnCompleteAsync()
        {
            return UniTask.CompletedTask;
        }
        
        public async UniTask<TResult> GetResultAsync(CancellationToken token)
        {
            StartAsync(token).Forget();
            
            var result = await _taskSource.Task;
            await OnCompleteAsync();
            
            return result;
        }

        private async UniTask StartAsync(CancellationToken token)
        {
            if (ExecutionToken != null && ExecutionToken.IsCancellationRequested)
            {
                throw new InvalidOperationException($"Attempt to get result from the disposed controller. Controller: {GetType().Name}");
            }

            ExecutionToken = CancellationTokenSource.CreateLinkedTokenSource(token, DisposeToken).Token;
            
            await OnStartAsync();
 
            if (ExecutionToken.IsCancellationRequested)
            {
                ReturnControllerResult();
            }
        }

        protected void ReturnControllerResult(TResult result = default)
        {
            if (!_taskSource.Task.Status.IsCompleted())
            {
                _taskSource.TrySetResult(result);
            }
        }
    }
}