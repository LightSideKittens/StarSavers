using LSCore;
using UnityEngine;

namespace StarSavers.Windows
{
    internal class MainWindow : BaseWindow<MainWindow>
    {
        [SerializeReference] private LSButtonAction[] buttonActions;

        protected override void Init()
        {
            base.Init();
            buttonActions.Invoke();
        }

        protected override void OnShowing()
        {
            LauncherWindowsData.CurrentShowedWindowIndex = 0;
            LauncherWindowsData.CurrentShowingWindowIndex = 0;
        }
    }
}