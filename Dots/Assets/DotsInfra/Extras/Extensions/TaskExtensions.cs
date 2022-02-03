using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dots.Extras
{
    public static class TaskExtensions
    {
        public static async UniTask WithFastCancellationSafe(this UniTask task, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            using (token.Register(() => tcs.TrySetResult(true)))
            {
                await UniTask.WhenAny(task, tcs.Task);
                if (task.Status.IsCompleted())
                {
                    await task;
                }
                else
                {
                    task.Forget();
                    Debug.Assert(tcs.Task.Status.IsCompleted(), "Task supposed to be completed in this case");
                    await tcs.Task;
                }
            }
        }

        public static async UniTask<T> WithFastCancellationSafe<T>(this UniTask<T> task, CancellationToken token,
            T cancelValue = default)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            using (token.Register(() => tcs.TrySetResult(true)))
            {
                await UniTask.WhenAny(task, tcs.Task);
                if (task.Status.IsCompleted())
                {
                    return await task;
                }

                task.Forget();
                Debug.Assert(tcs.Task.Status.IsCompleted(), "Task supposed to be completed in this case");
                return cancelValue;
            }
        }

        public static async UniTask<T> SkipCancelException<T>(this UniTask<T> target, T cancelValue = default)
        {
            try
            {
                await target;
            }
            catch (OperationCanceledException)
            {
            }

            return cancelValue;
        }

        public static async UniTask WithFastCancellation(this UniTask task, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource<object>();
            using (token.Register(() => tcs.TrySetCanceled()))
            {
                await UniTask.WhenAny(task, tcs.Task);
                if (task.Status.IsCompleted())
                {
                    await task;
                }
                else
                {
                    task.Forget();
                    UnityEngine.Debug.Assert(tcs.Task.Status.IsCompleted(),
                        "Task supposed to be completed in this case");
                    await tcs.Task;
                }
            }
        }

        public static async UniTask<T> WithFastCancellation<T>(this UniTask<T> task, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource<T>();
            using (token.Register(() => tcs.TrySetCanceled()))
            {
                await UniTask.WhenAny(task, tcs.Task);
                if (task.Status.IsCompleted())
                {
                    return await task;
                }

                task.Forget();
                UnityEngine.Debug.Assert(task.Status.IsCompleted(), "Task supposed to be completed in this case");
                return await tcs.Task;
            }
        }
    }
}