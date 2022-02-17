using Zenject;
using Object = UnityEngine.Object;

namespace Dots.AC
{
    public interface IInstantiateActorCommand
    {
        TActor Execute<TActor>(string instanceId = default)
            where TActor : class, IActor;
        TActor Execute<TActor>(IActor parent, bool worldPositionStays, string instanceId = default)
            where TActor : class, IActor;
    }

    /// <summary>
    /// Simple command to just instantiating registered actors.
    /// The main goal is to wrap of using an DI-container and provide convenient flexible testable api to instantiate actors
    /// Potentially can be used as an provider for remote instances  
    /// </summary>
    public class InstantiateActorCommand : IInstantiateActorCommand
    {
        private readonly DiContainer _container;

        public InstantiateActorCommand(DiContainer diContainer)
        {
            _container = diContainer;
        }

        public TActor Execute<TActor>(string instanceId = default)
            where TActor : class, IActor
        {
            var actor = instanceId == default
                ? _container.Resolve<TActor>()
                : _container.ResolveId<TActor>(instanceId);

            return Object.Instantiate(actor.Transform.gameObject).GetComponent<TActor>();
        }

        public TActor Execute<TActor>(IActor parent, bool worldPositionStays, string instanceId = default)
            where TActor : class, IActor
        {
            var actor = Execute<TActor>(instanceId);

            actor.SetParent(parent.Transform, worldPositionStays);

            return actor;
        }
    }
}