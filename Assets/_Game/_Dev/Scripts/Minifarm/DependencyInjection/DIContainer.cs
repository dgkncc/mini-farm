using System;
using System.Collections.Generic;
using Minifarm.Managers;
using UnityEngine;

namespace Minifarm.DependencyInjection
{
    public class DIContainer
    {
        private Dictionary<Type, object> instances = new Dictionary<Type, object>();

        public void Bind<T>(T instance) where T : class
        {
            var type = typeof(T);
            if (instances.ContainsKey(type))
            {
                DebugManager.Instance.Log($"Overriding existing binding for {type}",level: DebugManager.LogLevel.Warning);
            }
            instances[type] = instance;
        }

        public T Resolve<T>() where T : class
        {
            var type = typeof(T);
            if (instances.TryGetValue(type, out var instance))
            {
                return (T)instance;
            }

            DebugManager.Instance.Log($"No binding found for {type}",DebugManager.LogLevel.Error);
            return null;
        }

        public void Clear()
        {
            instances.Clear();
        }
    }
}