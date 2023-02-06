using System;
using UnityEngine;

namespace GameCore.CustomGameObject
{
    [Serializable]
    public class CustomGameObject
    {
        [SerializeField] protected GameObject gameObject;
        private bool isAwaked;
        private bool isStarted;
        private bool isActiveInHierarcy;

        public void OnParentAwake()
        {
            if (!isAwaked && gameObject.activeInHierarchy)
            {
                OnAwake();
                isAwaked = true;
            }
        }

        protected virtual void OnAwake()
        {
            Debug.Log($"[Malvis] OnAwake {gameObject.name}");
        }

        public void OnParentStart()
        {
            if (!isStarted && gameObject.activeInHierarchy)
            {
                OnStart();
                isStarted = true;
            }
        }
        
        protected virtual void OnStart()
        {
            Debug.Log($"[Malvis] OnStart {gameObject.name}");
        }

        public void OnParentEnable()
        {
            if (gameObject.activeInHierarchy && !isActiveInHierarcy)
            {
                Enable();
            }
        }
        
        protected virtual void OnEnable()
        {
            Debug.Log($"[Malvis] OnEnable {gameObject.name}");
        }
        
        public void OnParentDisable()
        {
            if (!gameObject.activeInHierarchy && isActiveInHierarcy)
            {
                Disable();
            }
        }
        
        protected virtual void OnDisable()
        {
            Debug.Log($"[Malvis] OnDisable {gameObject.name}");
        }

        public void OnParentDestroy()
        {
            Destroy();
        }
        
        protected virtual void OnDestroy()
        {
            Debug.Log($"[Malvis] OnDestroy {gameObject.name}");
        }

        public void SetActive(bool isActive)
        {
            var lastActiveInHierarchy = gameObject.activeInHierarchy;
            gameObject.SetActive(isActive);

            if (gameObject.activeInHierarchy != lastActiveInHierarchy)
            {
                if (lastActiveInHierarchy)
                {
                    Disable();
                }
                else
                {
                    Enable();
                }
            }
        }

        private void Enable()
        {
            OnParentAwake();
            OnEnable();
            isActiveInHierarcy = true;
        }

        private void Disable()
        {
            OnDisable();
            isActiveInHierarcy = false;
        }
        
        private void Destroy()
        {
            OnDestroy();
            isActiveInHierarcy = false;
        }
    }
}