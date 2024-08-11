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
        [SerializeField] private AnimSequencer fundTextAnim;
        [SerializeField] [Id(typeof(CurrencyIdGroup))] private Id fundFrom;
        [SerializeField] [Id(typeof(CurrencyIdGroup))] private Id fundTo;

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
            
            var fromAmount = Funds.GetValue(fundFrom);
            var toAmount = ExchangeTable.Convert(fundFrom, fundTo, fromAmount);
            Funds.Earn(fundTo, toAmount);
            
            var from = fundTextAnim.GetAnim<TextNumberAnim>("from");
            from.startValue = fromAmount;
            from.endValue = 0;
            var to = fundTextAnim.GetAnim<TextNumberAnim>("to");
            to.startValue = 0;
            to.endValue = toAmount;
            fundTextAnim.Init();
            Wait.Delay(1, () => fundTextAnim.Animate());
        }
    }
}