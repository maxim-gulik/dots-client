using UnityEngine;
using Zenject;

namespace DotsApp.Common
{
    [CreateAssetMenu(fileName = "InfrastructureInstaller", menuName = "Installers/InfrastructureInstaller")]
    public class InfrastructureInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {

        }
    }
}