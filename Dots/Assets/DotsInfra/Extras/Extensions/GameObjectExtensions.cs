using System.Linq;
using UnityEngine;

namespace Dots.Extras
{
    public static class GameObjectExtensions
    {
        public static TBase GetComponentOfBase<TBase>(this GameObject gameObject)
        {
            return gameObject.GetComponents<MonoBehaviour>().OfType<TBase>().FirstOrDefault();
        }

        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.name == default;
        }
    }
}