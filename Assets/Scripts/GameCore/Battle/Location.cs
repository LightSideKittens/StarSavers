using UnityEngine;

namespace GameCore.Battle
{
    public class Location : MonoBehaviour
    {
        [SerializeField] private Transform heroSpawnPoint;
        [SerializeField] private Transform mobsSpawnPoint;

        public Transform HeroSpawnPoint => heroSpawnPoint;
        public Transform MobsSpawnPoint => mobsSpawnPoint;
        
        
    }
}