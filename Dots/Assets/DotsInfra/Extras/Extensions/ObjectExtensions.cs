using System.Linq;

namespace Dots.Extras
{
	public static class ObjectExtensions
    {
        public static bool NotIn<T>(this T obj, params T[] set)
        {
            return !obj.In(set);
        }

        public static bool In<T>(this T obj, params T[] set)
        {
            return set != null && set.Contains(obj);
        }
    }
}
