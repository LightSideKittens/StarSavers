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
                var towers = MusicReactiveTest.Towers;
                for (int i = 0; i < towers.Length; i++)
                {
                    yield return towers[i];
                }
            }
        }

        protected override bool IsOpponent => true;

        [SerializeField] private Transform[] portals;
        [SerializeField] private Unit[] unitPrefabs;

        private void Start()
        {
            var shortTrack = MusicData.ShortTrackData;
            shortTrack.GetTrack(SoundventTypes.EnemyI).Started += () =>
            {
                Internal_Spawn(unitPrefabs[0], portals[Random.Range(0, portals.Length)].position);
            };
            
            shortTrack.GetTrack(SoundventTypes.EnemyII).Started += () =>
            {
                Internal_Spawn(unitPrefabs[1], portals[Random.Range(0, portals.Length)].position);
            };
            
            shortTrack.GetTrack(SoundventTypes.EnemyIII).Started += () =>
            {
                Internal_Spawn(unitPrefabs[2], portals[Random.Range(0, portals.Length)].position);
            };
            
            shortTrack.GetTrack(SoundventTypes.EnemyIV).Started += () =>
            {
                Internal_Spawn(unitPrefabs[3], portals[Random.Range(0, portals.Length)].position);
            };
        }
    }
}