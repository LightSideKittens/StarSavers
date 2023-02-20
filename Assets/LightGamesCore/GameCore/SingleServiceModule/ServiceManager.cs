using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Core.SingleService
{
    public abstract class ServiceManager : MonoBehaviour
    {
        private static readonly Dictionary<Type, BaseSingleService> Services = new Dictionary<Type, BaseSingleService>();

        [SerializeField] private List<BaseSingleService> services;

        protected virtual void Awake()
        {
            Debug.Log($"[ServiceManager] Awake. Scene: {SceneManager.GetActiveScene().name}");

            for (int i = 0; i < services.Count; i++)
            {
                var service = services[i];

                Services.Add(service.Type, service);
            }
        }

        protected virtual void OnDestroy()
        {
            Services.Clear();
        }

        internal static T GetService<T>() where T : BaseSingleService
        {
            CheckIsServiceExist<T>();
            return (T)Services[typeof(T)];
        }

        [Conditional("UNITY_EDITOR")]
        private static void CheckIsServiceExist<T>()
        {
            if (Services.ContainsKey(typeof(T)) == false)
            {
                var exeption = new Exception(
                    $"[{typeof(T)}] Check if the prefab with {typeof(T)} type is exist in the ServiceManager prefab" +
                    $" which should be on the scene.");

                exeption.Source = nameof(ServiceManager);
                
                throw exeption;
            }
        }
    }
}
