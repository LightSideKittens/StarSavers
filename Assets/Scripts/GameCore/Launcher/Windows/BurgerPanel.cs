using LSCore;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class BurgerPanel : BaseWindow<BurgerPanel>
    {
        [SerializeField] private Button backButton;
        protected override Button BackButton => backButton;
    }
}