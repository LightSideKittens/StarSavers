using BeatRoyale;
using Core.Extensions.Unity;
using GameCore.Battle.Data;
using static Battle.BattleBootstrap;
using static BeatRoyale.ShortNoteListener;
using static SoundventTypes;

namespace Battle
{
    public class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        private CardDecks decks;
        private readonly ShortNoteListener[] listeners = new ShortNoteListener[4];

        private void Start()
        {
            UserId = MatchData.OpponentUserId;
            decks = MatchData.GetDecks(UserId);
            listeners[0] = Listen(EnemyI).OnStarted(() => Spawn(0));
            listeners[1] = Listen(EnemyII).OnStarted(() => Spawn(1));
            listeners[2] = Listen(EnemyIII).OnStarted(() => Spawn(2));
            listeners[3] = Listen(EnemyIV).OnStarted(() => Spawn(3));
        }

        private void Spawn(int index)
        {
            Internal_Spawn(Units.ByName[decks.Defence[index]], OpponentSpawnArena.bounds.GetRandomPointInside());
        }

        protected override void OnStop()
        {
            for (int i = 0; i < Instance.listeners.Length; i++)
            {
                Instance.listeners[i].Dispose();
            }
        }
    }
}