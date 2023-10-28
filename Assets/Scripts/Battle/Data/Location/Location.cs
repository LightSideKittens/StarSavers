using LSCore.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Battle.Data
{
    public class Location : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject[] chunks;

        [Id("Enemies")] public int[] enemies;

        [Button]
        public void Generate()
        {
            var ground = Instantiate(prefab);
            const float step = 20f;
            var position = new Vector2(-100, 100);
            
            for (int x = 0; x < 10; x++)
            {
                position.y = 100;
                for (int y = 0; y < 10; y++)
                {
                    Instantiate(chunks.Random(), position, Quaternion.identity, ground.transform);
                    position.y -= step;
                }

                position.x += step;
            }
        }
    }
}