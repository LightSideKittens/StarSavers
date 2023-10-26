using LSCore;

namespace BeatHeroes.Windows
{
    public class ShopWindow : BaseWindow<ShopWindow>
    {
        protected override void OnShowing()
        {
            MainWindow.Hide();
        }

        protected override void OnBackButton()
        {
            base.OnBackButton();
            MainWindow.Show();
        }
    }
}