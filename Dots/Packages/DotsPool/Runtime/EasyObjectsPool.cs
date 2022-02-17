using MarchingBytes;
using UnityEngine;

namespace Dots.Pool
{
    /// <summary>
    /// Small service to wrap using of 3-rd party pool to make logic more testable and have the ability to replace solution
    /// </summary>
    public class EasyObjectsPool : IObjectsPool
    {
        private EasyObjectPool EasyObjectPool => EasyObjectPool.instance;

        public GameObject Get(string instanceId)
        {
            var obj = EasyObjectPool.GetObjectFromPool(instanceId, default, default);
            obj.transform.SetParent(EasyObjectPool.transform);

            return obj;
        }

        public T Get<T>(string instanceId) where T : Component
        {
            return Get(instanceId).GetComponent<T>();
        }

        public void Return<T>(T component) where T : Component
        {
            Return(component.gameObject);
        }

        public void Return(GameObject obj)
        {
            EasyObjectPool.ReturnObjectToPool(obj);
        }
    }
}