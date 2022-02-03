using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dots.Extras;

namespace Dots.Infra.AC.Utils
{
    public static class UniTaskUtils
    {
        public static UniTask FromEvent(Action<Action> subscribe, Action<Action> unSubscribe,            CancellationToken cancellationToken = default)
        {
            var taskSource = new UniTaskCompletionSource<object>();
            var registration = default(CancellationTokenRegistration);

            Action action = null;
            action = () =>
            {
                registration.Dispose();
                unSubscribe(action);
                taskSource.TrySetResult(null);
            };

            subscribe(action);

            registration = cancellationToken.Register(
                () =>
                {
                    if (taskSource.TrySetCanceled()) unSubscribe(action);
                });

            return taskSource.Task;
        }

        public static UniTask<T> FromEvent<T>(Action<Action<T>> subscribe, Action<Action<T>> unSubscribe, CancellationToken cancellationToken = default)
        {
            var taskSource = new UniTaskCompletionSource<T>();
            var registration = default(CancellationTokenRegistration);

            Action<T> action = null;
            action = result =>
            {
                registration.Dispose();
                unSubscribe(action);
                taskSource.TrySetResult(result);
            };

            subscribe(action);

            registration = cancellationToken.Register(
                () =>
                {
                    if (taskSource.TrySetCanceled()) unSubscribe(action);
                });

            return taskSource.Task;
        }

        public static UniTask FromEventSafe(Action<Action> subscribe, Action<Action> unSubscribe, CancellationToken cancellationToken)
        {
            var taskSource = new UniTaskCompletionSource<object>();
            var registration = default(CancellationTokenRegistration);

            void Action()
            {
                // ReSharper disable once AccessToModifiedClosure
                registration.Dispose();
                unSubscribe(Action);
                taskSource.TrySetResult(null);
            }

            subscribe(Action);

            registration = cancellationToken.Register(
                () =>
                {
                    if (taskSource.TrySetResult(null)) unSubscribe(Action);
                });

            return taskSource.Task;
        }

        public static UniTask<UniTask> FromEventSafe(Action<Func<UniTask>> subscribe, Action<Func<UniTask>> unSubscribe, CancellationToken cancellationToken)
        {
            var taskSource = new UniTaskCompletionSource<UniTask>();
            var registration = default(CancellationTokenRegistration);

            UniTask Func()
            {
                // ReSharper disable once AccessToModifiedClosure
                registration.Dispose();
                unSubscribe(Func);
                taskSource.TrySetResult(UniTask.CompletedTask);
                return UniTask.CompletedTask;
            }

            subscribe(Func);

            registration = cancellationToken.Register(
                () =>
                {
                    if (taskSource.TrySetResult(UniTask.CompletedTask)) unSubscribe(Func);
                });

            return taskSource.Task;
        }

        public static UniTask FromCallback(Action<Action> action, CancellationToken cancellationToken = default)
        {
            var taskSource = new UniTaskCompletionSource<object>();
            action(() => taskSource.TrySetResult(default));
            return taskSource.Task.WithFastCancellationSafe(cancellationToken);
        }

        public static UniTask<T> FromCallback<T>(Action<Action<T>> action, CancellationToken cancellationToken = default)
        {
            var taskSource = new UniTaskCompletionSource<T>();
            action(result => taskSource.TrySetResult(result));
            return taskSource.Task.WithFastCancellationSafe(cancellationToken);
        }

        public static UniTask FromCancellationToken(CancellationToken cancellationToken)
        {
            var taskCompletionSource = new UniTaskCompletionSource<object>();
            var registration = cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());
            taskCompletionSource.Task.ContinueWith(_ => { registration.Dispose(); });

            return taskCompletionSource.Task;
        }

        public static async UniTask WaitWhile(Func<bool> predicate, int checkDelay = 100,
            CancellationToken cancellationToken = default)
        {
            while (predicate.Invoke())
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                await UniTask.Delay(checkDelay, DelayType.Realtime, cancellationToken: cancellationToken);
            }
        }

        public static IEnumerable<UniTask> Select<T>(this IEnumerable<T> source, Func<T, UniTask> selector)
        {
            foreach (var x in source)
            {
                yield return selector.Invoke(x);
            }
        }
    }
}