using UnityEngine;

namespace Dots.Extras
{
    public static class GameObjectExtensions
    {
        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.name == default;
        }
    }
}