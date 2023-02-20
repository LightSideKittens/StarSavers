using System;
using UnityEngine;

namespace Core.Extensions.Unity
{
    public static class CameraExtensions
    {
        private static float aspect;
        private static Func<Camera, Vector3> sizeGetter;

        static CameraExtensions()
        {
            if (Screen.height > Screen.width)
            {
                aspect = (float)Screen.width / Screen.height;
                sizeGetter = GetSizeOnPortraitMode;
            }
            else
            {
                aspect = (float)Screen.height / Screen.width;
                sizeGetter = GetSizeOnAlbumMode;
            }
        }
    
        public static Bounds GetBounds(this Camera camera)
        {
            return new Bounds(camera.transform.position, sizeGetter(camera));
        }

        public static Vector3 GetSize(this Camera camera)
        {
            return sizeGetter(camera);
        }

        private static Vector3 GetSizeOnPortraitMode(Camera camera)
        {
            var orthographicSize = camera.orthographicSize;

            return new Vector3(orthographicSize, orthographicSize / aspect);
        }
    
        private static Vector3 GetSizeOnAlbumMode(Camera camera)
        {
            var orthographicSize = camera.orthographicSize;

            return new Vector3(orthographicSize / aspect, orthographicSize);
        }
    }
}
