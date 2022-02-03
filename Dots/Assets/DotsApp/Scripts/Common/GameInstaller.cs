using UnityEngine;
using Zenject;

namespace DotsApp.Common
{
    [CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/GameInstaller")]
    public class GameInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameRoot.Controller>().AsTransient();
        }
    }
}
