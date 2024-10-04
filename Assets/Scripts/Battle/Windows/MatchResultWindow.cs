using DG.Tweening;
using LSCore;
using LSCore.AnimationsModule;
using LSCore.AnimationsModule.Animations.Text;
using LSCore.Async;
using UnityEngine;

namespace Battle.Windows
{
    public class MatchResultWindow : BaseWindow<MatchResultWindow>
    {
        [SerializeField] private GameObject winState;
        [SerializeField] private GameObject loseState;

        public static void Show(bool isWin)
        {
            DOTween.KillAll();
            Show();
            PlayerWorld.Stop();
            OpponentWorld.Stop();
            Debug.Log("Stopped");
            Instance.Internal_Show(isWin);
        }

        private void Internal_Show(bool isWin)
        {
            winState.SetActive(isWin);
            loseState.SetActive(!isWin);
        }
    }
}