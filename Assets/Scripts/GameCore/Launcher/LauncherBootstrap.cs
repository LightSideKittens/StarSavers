using BeatRoyale.Windows;
using Core.SingleService;

namespace BeatRoyale.Launcher
{
    public class LauncherBootstrap : ServiceManager
    {
        protected override void Awake()
        {
            base.Awake();
            ControlPanel.Show();
        }
    }
}

