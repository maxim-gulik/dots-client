using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Dots.AC
{
    /// <summary>
    /// Root MonoBehaviour to start controller from a game object
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    [Serializable]
    public abstract class BaseSceneRootMonoBehavior<TController> : MonoBehaviour
        where TController : ISceneController
    {
        private readonly CancellationTokenSource _destroyTokenSource = new CancellationTokenSource();

        private ISceneController _controller;
        private ICreateControllerCommand _createControllerCommand;

        //Exception because of root instance. Injection into monobehaviours are prohibited
        [Inject]
        private void Construct(ICreateControllerCommand createControllerCommand)
        {
            _createControllerCommand = createControllerCommand;
        }

        private void Start()
        {
            _controller = CreateController();
            _controller.StartAsync().Forget();
        }

        private void OnDestroy()
        {
            _destroyTokenSource.Cancel();

            _controller?.Dispose(); //null check to cases when the scene was loaded but hasn't been activated
        }

        private void OnApplicationQuit()
        {
            StopSynchronizationContextWork(); //to avoid continuation of execution tasks
        }

        public ISceneController CreateController()
        {
            return _createControllerCommand.Execute<TController>();
        }

        // to avoid task continuation when play mode was turned off (unity has problems with it)
        [Conditional("UNITY_EDITOR")]
        private void StopSynchronizationContextWork()
        {
            var constructor = SynchronizationContext.Current.GetType()
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, binder: null, new[] { typeof(int) }, modifiers: null);
            var newContext = constructor.Invoke(new object[] { Thread.CurrentThread.ManagedThreadId });
            SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);
        }
    }
}