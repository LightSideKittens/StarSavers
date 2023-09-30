using Common.SingleServices.Windows;
using LSCore;
using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Battle.Windows
{
    public class MatchResultWindow : BaseWindow<MatchResultWindow>
    {
        [SerializeField] private GameObject winState;
        [SerializeField] private GameObject loseState;
        [SerializeField] private Button homeButton;

        public override int SortingOrder => 10;

        public static void Show(bool isWin)
        {
            Show();
            PlayerWorld.Stop();
            OpponentWorld.Stop();
            AnimatableWindow.Clean();
            Instance.Internal_Show(isWin);
        }

        private void Internal_Show(bool isWin)
        {
            winState.SetActive(isWin);
            loseState.SetActive(!isWin);
            homeButton.AddListener(OnHomeButton);
        }

        private void OnHomeButton()
        {
            SceneManager.LoadScene(0);
        }
    }
}