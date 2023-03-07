using System.Collections.Generic;
using GameCore.Battle.Data;
using MusicEventSystem.Configs;
using UnityEngine;

namespace Battle
{
    public class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        protected override IEnumerable<Transform> Targets
        {
            get
            {
                foreach (var tower in Tower.Towers)
                {
                    yield return tower;
                }
            }
        }

        protected override bool IsOpponent => true;

        [SerializeField] private Transform portal;
        [SerializeField] private Unit[] unitPrefabs;

        private void Start()
        {
            var shortTrack = MusicData.ShortTrackData;
            var spawnPosition = portal.position;
            shortTrack.GetTrack(SoundventTypes.EnemyI).Started += () =>
            {
                Internal_Spawn(unitPrefabs[0], spawnPosition);
            };
            
            shortTrack.GetTrack(SoundventTypes.EnemyII).Started += () =>
            {
                Internal_Spawn(unitPrefabs[1], spawnPosition);
            };
            
            shortTrack.GetTrack(SoundventTypes.EnemyIII).Started += () =>
            {
                Internal_Spawn(unitPrefabs[2], spawnPosition);
            };
            
            shortTrack.GetTrack(SoundventTypes.EnemyIV).Started += () =>
            {
                Internal_Spawn(unitPrefabs[3], spawnPosition);
            };
        }
    }
}