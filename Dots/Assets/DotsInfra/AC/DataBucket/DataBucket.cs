using System.Collections.Generic;
using System.Linq;

namespace Dots.Infra.AC
{
    public interface IDataBucket
    {
        void Put<T>(T value, string tag = "");
        bool TryTake<T>(out T value, string tag = "");
        bool TryTakeAnyOf<T>(out T value) where T : class;
    }

    public class DataBucket : IDataBucket
    {
        private readonly Dictionary<string, object> _storage = new Dictionary<string, object>();
        private readonly object _syncObject = new object();

        public void Put<T>(T value, string tag = "")
        {
            lock (_syncObject)
            {
                var key = GetKey<T>(tag);
                _storage[key] = value;
            }
        }

        public bool TryTake<T>(out T value, string tag = "")
        {
            lock (_syncObject)
            {
                var key = GetKey<T>(tag);
                if (_storage.TryGetValue(key, out var obj))
                {
                    _storage.Remove(key);

                    value = (T)obj;
                    return true;
                }

                value = default;
                return false;
            }
        }

        public bool TryTakeAnyOf<T>(out T value) where T : class
        {
            lock (_syncObject)
            {
                value = _storage.Values.OfType<T>().FirstOrDefault();
                return value != null;
            }
        }

        private static string GetKey<T>(string tag)
        {
            return $"{typeof(T)}_{tag}";
        }
    }
}