using System;
using Dots.Extras;
using UnityEngine;

namespace Dots.Infra.AC
{
    [RequireComponent(typeof(Transform))]
    public abstract class BaseActor : MonoBehaviour, IActor
    {
        private Transform _transform;
        private bool _destroyed = false;

        private readonly DisposablesContainer _disposableContainer = new DisposablesContainer();

        public Transform Transform => _transform ? _transform : _transform = gameObject.transform;

        public bool IsActive
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }

        private void OnDestroy()
        {
            _disposableContainer.Dispose();
            _destroyed = true;
        }

        public virtual void SetParent(Transform parent, bool worldPositionStays)
        {
            Transform.SetParent(parent, worldPositionStays);
        }

        public void Destroy()
        {
            if (!_destroyed)
                Destroy(gameObject);
        }

        public void AddOwnership(IDisposable disposable)
        {
            _disposableContainer.Add(disposable);
        }

        public void RemoveOwnership(IDisposable disposable, bool dispose = false)
        {
            _disposableContainer.Remove(disposable);

            if(dispose)
                disposable.Dispose();
        }
    }
}