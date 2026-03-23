using System;
using System.Collections.Generic;
namespace _02.Scripts.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
        }

        public static void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
        }

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"No service of type {typeof(T)}");
        }

        public static void Clear()
        {
            _services.Clear();
        }
    }
}