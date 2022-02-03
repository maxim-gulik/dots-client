using Dots.Infra.AC;
using UnityEngine;
using Zenject;

namespace Dots.Infra
{
    [CreateAssetMenu(fileName = "AppFrameworkSceneInstaller", menuName = "Installers/AppFrameworkScene")]
    public class AppFrameworkSceneInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ICreateControllerCommand>().To<CreateControllerCommand>().AsTransient();
            Container.Bind<IInstantiateActorCommand>().To<InstantiateActorCommand>().AsTransient();
            Container.Bind<IGetActorCommand>().To<GetActorCommand>().AsTransient();
        }
    }
}