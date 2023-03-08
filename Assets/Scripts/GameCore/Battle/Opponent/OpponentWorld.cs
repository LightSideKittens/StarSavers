using BeatRoyale;
using GameCore.Battle.Data;
using UnityEngine;
using static SoundventTypes;

namespace Battle
{
    public class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        protected override bool IsOpponent => true;

        [SerializeField] private Transform[] portals;
        [SerializeField] private Unit[] unitPrefabs;
        private int portalIndex;

        private void Start()
        {
            ShortNoteListener.Listen(EnemyI).Started += () => Spawn(0);
            ShortNoteListener.Listen(EnemyII).Started += () => Spawn(1);
            ShortNoteListener.Listen(EnemyIII).Started += () => Spawn(2);
            ShortNoteListener.Listen(EnemyIV).Started += () => Spawn(3);
        }

        private void Spawn(int index)
        {
            portalIndex++;
            portalIndex %= portals.Length;
            Internal_Spawn(unitPrefabs[index], portals[portalIndex].position);
        }
    }
}