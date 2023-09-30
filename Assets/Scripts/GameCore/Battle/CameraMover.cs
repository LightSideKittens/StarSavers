using UnityEngine;

namespace GameCore.Battle
{
    public static class CameraMover
    {
        public static Camera Camera { get; private set; }
        public static Transform Hero { get; private set; }
        private static Vector3 prevHeroPosition;
        
        public static void Init(Camera camera, Transform hero)
        {
            Camera = camera;
            Hero = hero;
            prevHeroPosition = Hero.position;
        }

        public static void MoveCamera()
        {
            var offset = Hero.position - prevHeroPosition;
            Camera.transform.position += new Vector3(offset.x, offset.y, 0);
            prevHeroPosition = Hero.position;
        }
    }
}