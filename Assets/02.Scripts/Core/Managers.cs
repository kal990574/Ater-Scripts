using System;
using System.Collections.Generic;
namespace _02.Scripts.Core
{
    public static class Managers
    {
        private static readonly Dictionary<Type, object> _managers = new();

        public static void Register<T>(T service) where T : class
        {
            _managers[typeof(T)] = service;
        }

        public static void Unregister<T>() where T : class
        {
            _managers.Remove(typeof(T));
        }

        public static T Get<T>() where T : class
        {
            if (_managers.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"No service of type {typeof(T)}");
        }

        public static void Clear()
        {
            _managers.Clear();
        }
    }
}