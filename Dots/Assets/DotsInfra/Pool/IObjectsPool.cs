using UnityEngine;

namespace DotsInfra.Pool
{
    public interface IObjectsPool
    {
        GameObject Get(string instanceId);
        T Get<T>(string instanceId) where T : Component;
        void Return<T>(T component) where T : Component;
        void Return(GameObject gameObject);
    }
}