using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dots.Infra.Assets;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Dots.Infra.AC
{
    public interface IInstantiateActorCommand
    {
        TActor Execute<TActor>(string instanceId = default)
            where TActor : class, IActor;
        TActor Execute<TActor>(IActor parent, bool worldPositionStays, string instanceId = default)
            where TActor : class, IActor;
        UniTask<TActor> ExecuteAsync<TActor>(string assetId, string assetGroup, CancellationToken token)
            where TActor : class, IActor;
        UniTask<TActor> ExecuteAsync<TActor>(string assetId, string assetGroup, IActor parent, bool worldPositionStays, CancellationToken token)
            where TActor : class, IActor;
    }

    public class InstantiateActorCommand : IInstantiateActorCommand
    {
        private readonly IAssetLoader _assetLoader;
        private readonly DiContainer _container;

        public InstantiateActorCommand(
            IAssetLoader assetLoader,
            DiContainer diContainer)
        {
            _assetLoader = assetLoader;
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

        public async UniTask<TActor> ExecuteAsync<TActor>(string assetId, string assetGroup, CancellationToken token)
            where TActor : class, IActor
        {
            var instanceType = _container.ResolveType<TActor>();

            var handler = await _assetLoader.LoadAsync<GameObject>(assetId, assetGroup, token);
            if (handler == null)
            {
                Debug.LogWarning($"Actor was not loaded. AssetId: {assetId}, AssetGroup: {assetGroup}, ActorType: {instanceType.Name}");
                return null;
            }

            var actor = Object.Instantiate(handler.Asset).GetComponent(instanceType) as TActor;
            if (actor == null)
                throw new InvalidOperationException($"Couldn't find actor in the loaded gameobject. ActorType:{instanceType.Name}, AssetId:{assetId}, AssetGroup: {assetGroup}");

            actor.AddOwnership(handler);

            return actor;
        }

        public TActor Execute<TActor>(IActor parent, bool worldPositionStays, string instanceId = default)
            where TActor : class, IActor
        {
            var actor = Execute<TActor>(instanceId);

            actor.SetParent(parent.Transform, worldPositionStays);

            return actor;
        }

        public async UniTask<TActor> ExecuteAsync<TActor>(string assetId, string assetGroup, IActor parent, bool worldPositionStays, CancellationToken token)
            where TActor : class, IActor
        {
            var actor = await ExecuteAsync<TActor>(assetId, assetGroup, token);
            if (actor == null)
                return null;

            actor.SetParent(parent.Transform, worldPositionStays);
            return actor;
        }
    }
}