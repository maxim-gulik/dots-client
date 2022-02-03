using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

namespace Dots.Infra.AC
{
    [Serializable]
    public abstract class BaseSceneRootMonoBehavior : MonoBehaviour
    {
        private readonly CancellationTokenSource _destroyTokenSource = new CancellationTokenSource();

        private ISceneController _controller;

        protected abstract string SceneId { get; }

        public abstract ISceneController CreateController();

        private void Start()
        {
            Debug.Log($"[{SceneId}] scene activated.");

            _controller = CreateController();
            _controller.StartAsync().Forget();
        }

        private void OnDestroy()
        {
            Debug.Log($"[{SceneId}] scene deactivated.");

            _destroyTokenSource.Cancel();

            _controller?.Dispose(); //null check to cases when the scene was loaded but hasn't been activated
        }

        private void OnApplicationQuit()
        {
            StopSynchronizationContextWork(); //to avoid continuation of execution tasks
        }

        [Conditional("UNITY_EDITOR")]
        private void StopSynchronizationContextWork()
        {
            var constructor = SynchronizationContext.Current.GetType()
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, binder: null, new[] { typeof(int) }, modifiers: null);
            var newContext = constructor.Invoke(new object[] { Thread.CurrentThread.ManagedThreadId });
            SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);
        }
    }

    [Serializable]
    public abstract class BaseSceneRootMonoBehavior<TController> : BaseSceneRootMonoBehavior
        where TController : ISceneController
    {
        private ICreateControllerCommand _createControllerCommand;

        [Inject]
        private void Construct(ICreateControllerCommand createControllerCommand)
        {
            _createControllerCommand = createControllerCommand;
        }

        public sealed override ISceneController CreateController()
        {
            return _createControllerCommand.Execute<TController>();
        }
    }

    [Serializable]
    public abstract class BaseSceneRootMonoBehavior<TController, TParameters> : BaseSceneRootMonoBehavior
        where TController : ISceneController
        where TParameters : class
    {
        private ICreateControllerCommand _createControllerCommand;
        private IDataBucket _dataBucket;

        [Inject]
        private void Construct(
            IDataBucket dataBucket,
            ICreateControllerCommand createControllerCommand)
        {
            _createControllerCommand = createControllerCommand;
            _dataBucket = dataBucket;
        }

        public sealed override ISceneController CreateController()
        {
            if (_dataBucket.TryTake(out TParameters parameters))
            {
                return _createControllerCommand.Execute<TController, TParameters>(parameters);
            }

            throw new InvalidOperationException($"Parameters was not passed scene controller. Controller: {GetType().Name}, Parameters: {typeof(TParameters).Name}");
        }
    }
}