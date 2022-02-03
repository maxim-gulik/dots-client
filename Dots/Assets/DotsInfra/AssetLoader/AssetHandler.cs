using System;
using Object = UnityEngine.Object;

namespace Dots.Infra.Assets
{
    public interface IAssetHandler<T> : IDisposable
        where T : Object
    {
        T Asset { get; }
    }

    public class AssetHandler<T> : IAssetHandler<T>
        where T : Object
    {
        private readonly Action _releaseAction;
        private readonly string _id;
        private readonly string _group;

        private T _asset;

        public AssetHandler(string id, string group, T asset, Action releaseAction)
        {
            _releaseAction = releaseAction;
            _id = id;
            _group = group;
            _asset = asset;
        }

        public T Asset
        {
            get
            {
                if (_asset == null)
                    throw new InvalidOperationException($"Attempt to get a released asset. Type: {nameof(T)}, Id:{_id}, Group: {_group}");

                return _asset;
            }
        }

        public void ReleaseAsset()
        {
            if (_asset == null)
                return;

            _releaseAction?.Invoke();
            _asset = null;
        }

        public void Dispose()
        {
            ReleaseAsset();
        }
    }
}