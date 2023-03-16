using System;
using Common.SingleServices.Windows;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.SingleServices
{
    [Serializable]
    public class Loader
    {
        [SerializeField] private GameObject gameObject;

        public static Loader Create()
        {
            var spawnPoint = AnimatableWindow.SpawnPoint;
            var animText = AnimatableWindow.Loader;

            var scale = animText.gameObject.transform.localScale;
            var obj = Object.Instantiate(animText.gameObject, spawnPoint.parent);
            obj.transform.localScale = scale;

            return new Loader
            {
                gameObject = obj,
            };
        }

        public void Destroy()
        {
            Object.Destroy(gameObject);
        }
    }
}