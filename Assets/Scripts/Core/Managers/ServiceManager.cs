using System.Collections.Generic;
using Core.Ordinaries;
using UnityEngine;

namespace Core.Managers
{
    public interface IService
    {
        public void Initialize();
    }

    public class ServiceManager : Singleton<ServiceManager>
    {
        private readonly Dictionary<string, IService> _registeredServices = new();

        public bool RegisterService<T>(T service) where T : IService
        {
            var serviceKey = typeof(T).Name;
            var isSuccess = _registeredServices.TryAdd(serviceKey, service);

            if (isSuccess)
            {
                service.Initialize();
            }

            return isSuccess;
        }

        public T GetService<T>() where T : IService
        {
            var serviceKey = typeof(T).Name;
            var isSuccess = _registeredServices.TryGetValue(serviceKey, out var service);

            if (isSuccess)
            {
                return (T)service;
            }

            Debug.LogError($"Service {serviceKey} not registered.");
            return default;
        }

        public bool RemoveService<T>() where T : IService
        {
            var serviceKey = typeof(T).Name;
            var isSuccess = _registeredServices.Remove(serviceKey);

            return isSuccess;
        }
    }
}