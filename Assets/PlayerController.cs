using UnityEngine;
using UnityEngine.AI;

namespace BeatHeroes
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        private NavMeshAgent agent;
        
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }
        
        void Update()
        {
            agent.SetDestination(player.position);
        }
    }
}
