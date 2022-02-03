using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dots.Infra.Assets
{
    public interface IAssetLoader
    {
        public UniTask<IAssetHandler<T>> LoadAsync<T>(string id, CancellationToken token)
            where T : Object;
        UniTask<IAssetHandler<T>> LoadAsync<T>(string id, string group, CancellationToken token)
            where T : Object;
    }
}