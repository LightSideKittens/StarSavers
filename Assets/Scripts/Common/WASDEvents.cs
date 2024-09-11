using System;
using UnityEngine;

namespace StarSavers
{
    public class WASDEvents : MonoBehaviour
    {
        public static event Action Forward;
        public static event Action ForwardLeft;
        public static event Action ForwardRight;
        public static event Action Back;
        public static event Action BackLeft;
        public static event Action BackRight;
        public static event Action Right;
        public static event Action Left;
        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    ForwardLeft?.Invoke();
                    return;
                }
                
                if (Input.GetKey(KeyCode.D))
                {
                    ForwardRight?.Invoke();
                    return;
                }
                
                Forward?.Invoke();
                return;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    BackLeft?.Invoke();
                    return;
                } 
                
                if (Input.GetKey(KeyCode.D))
                {
                    BackRight?.Invoke();
                    return;
                }
                
                Back?.Invoke();
                return;
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                Left?.Invoke();
                return;
            } 
                
            if (Input.GetKey(KeyCode.D))
            {
                Right?.Invoke();
                return;
            }
        }
    }
}
