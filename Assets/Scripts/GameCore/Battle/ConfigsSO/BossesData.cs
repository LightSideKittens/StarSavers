using UnityEngine;

namespace Battle.ConfigsSO
{
    [CreateAssetMenu(fileName = "BossesData", menuName = "Battle/BossesData", order = 0)]
    public class BossesData : ScriptableObject
    {
        [SerializeField] private EnemiesData.Data data;
        public static EnemiesData.Data Data { get; private set; }

        public void Init()
        {
            Data = data;
        }
    }
}