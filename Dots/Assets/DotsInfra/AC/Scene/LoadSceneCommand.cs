using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Dots.Infra.AC;
using Dots.Infra.Utils;

public interface ILoadSceneCommand
{
    UniTask<bool> ExecuteAsync(string name, LoadSceneMode mode, CancellationToken token);
    UniTask<bool> ExecuteAsync(string name, LoadSceneMode mode, Action<float> onProgress, CancellationToken token);
    UniTask<bool> ExecuteAsync<TParameters>(string name, TParameters parameters, LoadSceneMode mode, Action<float> onProgress, CancellationToken token);
}

public class LoadSceneCommand : ILoadSceneCommand
{
    private const float SceneLoadedProgressInAccordanceWithUnity = 0.9f;

    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IDataBucket _dataBucket;

    public LoadSceneCommand(
        ICoroutineRunner coroutineRunner,
        IDataBucket dataBucket)
    {
        _coroutineRunner = coroutineRunner;
        _dataBucket = dataBucket;
    }

    public UniTask<bool> ExecuteAsync(string name, LoadSceneMode mode, Action<float> onProgress, CancellationToken token)
    {
        var taskSource = new UniTaskCompletionSource<bool>();

        _coroutineRunner.StartCoroutine(
            LoadingCoroutine(
                name,
                mode,
                onComplete: r => taskSource.TrySetResult(r),
                onProgress,
                token));

        return taskSource.Task;
    }

    public UniTask<bool> ExecuteAsync<TParameters>(string name, TParameters parameters, LoadSceneMode mode, Action<float> onProgress, CancellationToken token)
    {
        var taskSource = new UniTaskCompletionSource<bool>();

        _coroutineRunner.StartCoroutine(
            LoadingCoroutine(
                name,
                mode,
                onComplete: r => taskSource.TrySetResult(r),
                onProgress,
                token));

        _dataBucket.Put(parameters);
        return taskSource.Task;
    }

    public UniTask<bool> ExecuteAsync(string name, LoadSceneMode mode, CancellationToken token)
    {
        var taskSource = new UniTaskCompletionSource<bool>();

        _coroutineRunner.StartCoroutine(
            LoadingCoroutine(
                name,
                mode,
                onComplete: r => taskSource.TrySetResult(r),
                _ => { },
                token));

        return taskSource.Task;
    }

    private IEnumerator LoadingCoroutine(string name, LoadSceneMode mode, Action<bool> onComplete, Action<float> onProgress, CancellationToken token)
    {
        var operation = SceneManager.LoadSceneAsync(name, mode);

        onProgress(0);

        var progress = 0f;
        while (true)
        {
            if (operation.progress > progress)
            {
                progress = operation.progress;
                onProgress(progress);
            }

            yield return null;

            if (operation.progress >= SceneLoadedProgressInAccordanceWithUnity)
            {
                onProgress(1);

                onComplete(true);
                yield break;
            }

            if (token.IsCancellationRequested)
            {
                onComplete(false);
                yield break;
            }
        }
    }
}
