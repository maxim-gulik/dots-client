using System;
using System.Diagnostics;
using Zenject;
using Dots.Extras;

namespace Dots.Infra.AC
{
    public interface IGetActorCommand
    {
        TActor Execute<TActor>(string instanceId = default) where TActor : IActor;
    }

    /// <summary>
    /// Simple command to just getting registered actors.
    /// The main goal is to wrap of using an DI-container and provide convenient testable api to get actors 
    /// </summary>
    public class GetActorCommand : IGetActorCommand
    {
        private readonly DiContainer _container;

        public GetActorCommand(DiContainer diContainer)
        {
            _container = diContainer;
        }

        public TActor Execute<TActor>(string instanceId = default) where TActor : IActor
        {
            var actor = instanceId == default ? _container.Resolve<TActor>() : _container.ResolveId<TActor>(instanceId);

            CheckIsActorValid(actor, instanceId);

            return actor;
        }

        [Conditional("UNITY_EDITOR")]
        private void CheckIsActorValid<TActor>(TActor actor, string instanceId) where TActor : IActor
        {
            if (actor == null)
            {
                var text = $"Null instance of game actor. Check binding settings. Actor: {typeof(TActor).Name}";
                if (instanceId != null)
                {
                    text += $", InstanceId: {instanceId}";
                }
                throw new InvalidOperationException(text);
            }
            
            var gameObject = actor.Transform.gameObject;
            if (gameObject.IsPrefab())
            {
                throw new InvalidOperationException($"Attempt to catch the actor from the prefab. Check binding settings. Actor: {actor.GetType().Name}, Prefab: {gameObject.name}");
            }
        }
    }
}