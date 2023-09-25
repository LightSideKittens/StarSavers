using GameCore.Battle.Data;
using LGCore.Extensions.Unity;
using static Battle.BattleWorld;

namespace Battle
{
    public class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        private CardDecks decks;

        private void Start()
        {
            UserId = "Opponent";
        }

        private void Spawn(int index)
        {
            Internal_Spawn(Units.ByName[decks.Defence[index]], OpponentSpawnArena.bounds.GetRandomPointInside());
        }

        protected override void OnStop()
        {
            
        }
    }
}