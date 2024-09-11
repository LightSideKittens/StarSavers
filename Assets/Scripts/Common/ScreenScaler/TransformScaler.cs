using UnityEngine;

namespace StarSavers
{
    public class TransformScaler : BaseScreenScaler
    {
        [SerializeField] private Vector3 baseScale = new(1, 1, 1);
        protected override void Scale()
        {
            transform.localScale = baseScale * Factor;
        }
    }
}