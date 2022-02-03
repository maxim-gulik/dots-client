using Dots.Infra.AC;
using Dots.Infra.Assets;
using Dots.Infra.Utils;
using UnityEngine;
using Zenject;

namespace DotsApp.Common
{
    [CreateAssetMenu(fileName = "InfrastructureInstaller", menuName = "Installers/InfrastructureInstaller")]
    public class InfrastructureInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IMessageBus>().To<MessageBus>().AsSingle();
            Container.Bind<IDataBucket>().To<DataBucket>().AsSingle();

            Container.Bind<ILoadSceneCommand>().To<LoadSceneCommand>().AsTransient();
            Container.Bind<IUnloadSceneCommand>().To<UnloadSceneCommand>().AsTransient();

            Container.Bind<IAssetLoader>().To<AddressablesAssetLoader>().AsTransient();

            Container.Bind<ICreateControllerCommand>().To<CreateControllerCommand>().AsTransient();
            Container.Bind<IInstantiateActorCommand>().To<InstantiateActorCommand>().AsTransient();
            Container.Bind<IGetActorCommand>().To<GetActorCommand>().AsTransient();

            Container.Bind(typeof(IApplicationSupport), typeof(IApplicationObserver), typeof(ICoroutineRunner))
                .To<ApplicationSupport>()
                .FromNewComponentOnNewGameObject()
                .AsSingle();
        }
    }
}