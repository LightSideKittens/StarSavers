using UnityEngine;

namespace StarSavers
{
    [ExecuteAlways]
    public abstract class BaseScreenScaler : MonoBehaviour
    {
        [SerializeField] protected bool invert;
        [SerializeField] protected Vector2 reference = new(1080, 1920);
        
        protected float Factor
        {
            get
            {
                var refAspect = reference.y / reference.x;
                var currentAspect = (float) Screen.height / Screen.width;
                var totalAspect = invert ? refAspect / currentAspect : currentAspect / refAspect;

                return currentAspect > refAspect ? totalAspect : 1f;
            }
        }
        
        private void Start()
        {
            Scale();
            enabled = !Application.isPlaying;
        }

        private void Update()
        {
            Scale();
        }

        protected abstract void Scale();
    }
}