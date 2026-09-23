using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Infrastructure
{
    public static class G
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            Type type = typeof(T);
            if (!_services.TryAdd(type, service)) Debug.LogError($" the {type.Name} type has already registered");
        }

        public static void UnRegister<T>()
        {
            Type type = typeof(T);
            if (!_services.ContainsKey(type))
            {
                Debug.LogError($"you trying to unregister non-existent {type.Name} service");
                return;
            }

            _services.Remove(type);
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type,out var value))
                Debug.LogError($"you trying to get unregistred {type.Name} service");

            return (T) value;
        }

        public static void Clear() => 
            _services.Clear();
    }
}