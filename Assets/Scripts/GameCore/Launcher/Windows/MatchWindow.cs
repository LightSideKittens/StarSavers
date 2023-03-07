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
            MatchData.Clear();
            var opponentPlayerData = new MatchData.PlayerData();

            
            RemotePlayerData<CardDecks>.Fetch(opponentUserId, decks =>
            {
                opponentPlayerData.decks = decks;
                RemotePlayerData<EntitiesProperties>.Fetch(opponentUserId, properties =>
                {
                    opponentPlayerData.properties = properties;
                    MatchData.PlayerDataByUserId.Add(opponentUserId, opponentPlayerData);
                    SceneManager.LoadScene(1);
                });
            });
        }
    }
}