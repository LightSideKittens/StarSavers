using System.Collections;
using System.Collections.Generic;
using GameCore.Battle;
using UnityEngine;

namespace BeatRoyale
{
    public class ElipsDrawer2 : MonoBehaviour
    {
        [SerializeField] private Vector2 offset;
        [SerializeField] private float speed = 5;
        [SerializeField] private float scale = 1;
        private float time;
        
        void Update()
        {
            time += Time.deltaTime * speed;
            var position = new Vector2(Mathf.Sin(time), Mathf.Cos(time));
            RadiusUtils.ToPerspective(ref position, scale);
            transform.localPosition = position;
        }
    }
}
