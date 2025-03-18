using System;
using System.Collections.Generic;
using Code.Scripts.Services.Common;

namespace Code.Scripts.App.Common
{
    public class ServiceLocator : IDisposable
    {
        private readonly List<IService> _services = new();

        public void Register(IService service)
        {
            _services.Add(service);
        }
        
        public bool Unregister(IService service)
        {
            return _services.Remove(service);
        }

        public T Get<T>()
        {
            for (var i = 0; i < _services.Count; i++)
            {
                if (_services[i] is T service)
                {
                    return service;
                }
            }

            return default;
        }
        
        public bool TryGet<T>(out T service)
        {
            for (var i = 0; i < _services.Count; i++)
            {
                if (_services[i] is T result)
                {
                    service = result;
                    return true;
                }
            }

            service = default;
            return false;
        }
        
        public void CollectAll<T>(List<T> result)
        {
            for (var i = 0; i < _services.Count; i++)
            {
                if (_services[i] is T service)
                {
                    result.Add(service);
                }
            }
        }

        public void Clear()
        {
            for (var i = 0; i < _services.Count; i++)
            {
                if (_services[i] is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            _services.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}