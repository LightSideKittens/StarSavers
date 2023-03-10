using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatRoyale
{
    public class ElipsDrawer : MonoBehaviour
    {

        [SerializeField] private Vector2 offset;
        [SerializeField] private float speed = 5;
        [SerializeField] private float scale = 1;
        private float time;
        void Update()
        {
            time += Time.deltaTime * speed;
            transform.position = new Vector2(Mathf.Sin(time) * 2 * scale, Mathf.Cos(time) * 1.42f * scale) + offset;
        }
    }
}
