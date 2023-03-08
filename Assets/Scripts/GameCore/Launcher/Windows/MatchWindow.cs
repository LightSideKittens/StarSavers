using Battle.Data;
using Core.ConfigModule;
using Core.Extensions.Unity;
using GameCore.Battle.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class MatchWindow : BaseLauncherWindow<MatchWindow>
    {
        [SerializeField] private string opponentUserId;
        [SerializeField] private Button matchButton;
        protected override int Internal_Index => 2;

        protected override void Init()
        {
            base.Init();
            matchButton.AddListener(Match);
        }

        private void Match()
        {
            MatchPlayersData.Clear();
            
            RemotePlayerData<CardDecks>.Fetch(opponentUserId, decks =>
            {
                RemotePlayerData<EntitiesProperties>.Fetch(opponentUserId, properties =>
                {
                    MatchPlayersData.Add(opponentUserId, decks, properties);
                    SceneManager.LoadScene(1);
                });
            });
        }
    }
}