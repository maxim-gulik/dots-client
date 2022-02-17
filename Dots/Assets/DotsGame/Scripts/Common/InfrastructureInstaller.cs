using Dots.AC;
using Dots.Extras;
using Dots.Pool;
using UnityEngine;
using Zenject;

namespace DotsGame
{
    /// <summary>
    /// Zenject installer only for infrastructure injections 
    /// </summary>
    [CreateAssetMenu(fileName = "InfrastructureInstaller", menuName = "Installers/InfrastructureInstaller")]
    public class InfrastructureInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ICreateControllerCommand>().To<CreateControllerCommand>().AsTransient();
            Container.Bind<IInstantiateActorCommand>().To<InstantiateActorCommand>().AsTransient();
            Container.Bind<IGetActorCommand>().To<GetActorCommand>().AsTransient();

            Container.Bind<IObjectsPool>().To<EasyObjectsPool>().AsSingle();

            Container.Bind(typeof(IApplicationSupport), typeof(IFramePulse))
                .To<ApplicationSupport>()
                .FromNewComponentOnNewGameObject()
                .AsSingle();
        }
    }
}