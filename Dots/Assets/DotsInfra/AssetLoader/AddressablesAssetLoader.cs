using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Dots.Infra.Assets
{
    public class AddressablesAssetLoader : IAssetLoader
    {
        private const string DefaultGroup = null;

        public UniTask<IAssetHandler<T>> LoadAsync<T>(string id, CancellationToken token)
            where T : Object
        {
            return LoadAsync<T>(id, DefaultGroup, token);
        }

        public async UniTask<IAssetHandler<T>> LoadAsync<T>(string id, string group, CancellationToken token)
            where T : Object
        {
            var operationHandle = Addressables.LoadAssetAsync<T>(id);
            await operationHandle;

            if (operationHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogWarning($"Error during loading asset. AssetId: {id}, AssetGroup: {group}, Type: {nameof(T)}, ErrorMessage: {operationHandle.OperationException.Message}");
                return null;
            }

            if (token.IsCancellationRequested)
            {
                ReleaseAsset();
                return null;
            }

            return new AssetHandler<T>(id, group, operationHandle.Result, ReleaseAsset);

            void ReleaseAsset() => Addressables.Release(operationHandle);
        }
    }
}
