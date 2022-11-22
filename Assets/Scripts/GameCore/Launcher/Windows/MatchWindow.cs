using Core.Extensions.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class MatchWindow : BaseWindow<MatchWindow>
    {
        [SerializeField] private Button matchButton;
        
        protected override void Init()
        {
            base.Init();
            matchButton.AddListener(Match);
        }

        private void Match()
        {
            SceneManager.LoadScene(1);
        }
    }
}