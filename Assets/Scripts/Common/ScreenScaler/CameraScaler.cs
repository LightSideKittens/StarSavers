using UnityEngine;

namespace StarSavers
{
    public class CameraScaler : BaseScreenScaler
    {
        [SerializeField] private Vector3 cameraPosition;
        [SerializeField] private Vector3 offset = new(0, 4);
        [SerializeField] private float referenceSize = 5;

        protected override void Scale()
        {
            var factor = Factor;
            var camera = Camera.main;
            camera.orthographicSize = referenceSize * factor;
            camera.transform.position = cameraPosition + offset * (factor - 1);
        }
    }
}