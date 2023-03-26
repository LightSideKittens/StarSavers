using Common.SingleServices;
using Core.Extensions.Unity;
using Core.Server;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class MatchWindow : BaseLauncherWindow<MatchWindow>
    {
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
            var loader = Loader.Create();
            Leaderboards.GetUserId(opponentUserId =>
            {
                MatchData.Add(opponentUserId, () =>
                {
                    loader.Destroy();
                    SceneManager.LoadScene(1);
                });
            });
        }
    }
}