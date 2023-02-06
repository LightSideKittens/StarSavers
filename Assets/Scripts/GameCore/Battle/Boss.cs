using Battle.ConfigsSO;
using UnityEngine;

namespace Battle
{
    public class Boss : Enemy
    {
        public override Vector3 EndPosition => Position - new Vector3(0, 5, 0);
        
        protected override EnemiesData.Data GetEnemy()
        {
            return BossesData.Data;
        }

        protected override void OnMoveCompleted() { }
    }
}