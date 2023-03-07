using System;
using UnityEngine;

namespace Core.SingleService
{
    public abstract class SingleService<T> : BaseSingleService where T : SingleService<T>
    {
        private static T instance;
        private static Func<T> staticConstructor;
        private static Action onInitializing;
        public override Type Type => typeof(T);

        protected static T Instance => staticConstructor();

        static SingleService()
        {
            staticConstructor = StaticConstructor;
        }

        private static T StaticConstructor()
        {
            onInitializing?.Invoke();
            staticConstructor = TrowException;

            if (instance == null)
            {
                instance = Instantiate(ServiceManager.GetService<T>());
            }

            staticConstructor = GetInstance;

            instance.Init();

            return instance;
        }

        private static T GetInstance() => instance;

        private static T TrowException() => throw new Exception(
            $" You try get {nameof(Instance)} before initializing." +
            $" Use {nameof(Init)} method by override in {typeof(T)} class.");

        public static void AddChild(Transform child, bool worldPositionStays = false)
        {
            child.SetParent(Instance.transform, worldPositionStays);
        }
        
        public static void SetParent<TParent>(bool worldPositionStays = false) where TParent : SingleService<TParent>
        {
            SingleService<TParent>.AddChild(Instance.transform, worldPositionStays);
        }
        
        protected static void OnInitializing(Action action)
        {
            onInitializing = action;
        }
    
        protected static void AddOnInitializing(Action action)
        {
            onInitializing += action;
        }

        protected virtual void Awake()
        {
            instance = (T)this;
            Debug.Log($"[{GetType().Name}] Awake");
        }

        protected virtual void Init() { }

        protected virtual void OnDestroy()
        {
            onInitializing = null;
            staticConstructor = StaticConstructor;
        }
    }
}