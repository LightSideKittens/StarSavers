using Sirenix.OdinInspector;
using UnityEngine;

namespace BeatHeroes
{
    public class TransformScale : MonoBehaviour
    {
        [Button("Execute")]
        public void Scale()
        {
            var transforms = GetComponentsInChildren<Transform>();

            foreach (var tr in transforms)
            {
                Vector2 temp = tr.position;
                temp.x /= 32;
                temp.y /= 32;
                tr.position = temp;
            }
        }
    }
}
