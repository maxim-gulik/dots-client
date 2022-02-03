using System;
using Dots.Infra.AC;
using Dots.Infra.Assets;
using Dots.Infra.Utils;
using UnityEngine;
using Zenject;

namespace Dots.Infra
{
    [CreateAssetMenu(fileName = "AppFrameworkProjectInstaller", menuName = "Installers/AppFrameworkProject")]
    public class AppFrameworkProjectInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private RemoteAssetsSystem _assetsControlSystem;

        public override void InstallBindings()
        {
            Container.Bind<IMessageBus>().To<MessageBus>().AsSingle();
            Container.Bind<IDataBucket>().To<DataBucket>().AsSingle();

            Container.Bind<ILoadSceneCommand>().To<LoadSceneCommand>().AsTransient();
            Container.Bind<IUnloadSceneCommand>().To<UnloadSceneCommand>().AsTransient();

            Container.Bind(typeof(IApplicationSupport), typeof(IApplicationObserver), typeof(ICoroutineRunner))
                .To<ApplicationSupport>()
                .FromNewComponentOnNewGameObject()
                .AsSingle();

            if (_assetsControlSystem == RemoteAssetsSystem.Addressable)
                Container.Bind<IAssetLoader>().To<AddressablesAssetLoader>().AsTransient();
            else
                throw new ApplicationException($"Undefined assets control system: {_assetsControlSystem}");
        }

        public enum RemoteAssetsSystem
        {
            Addressable
        }
    }
}
