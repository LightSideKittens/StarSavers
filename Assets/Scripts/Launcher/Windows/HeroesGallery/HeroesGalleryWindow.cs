namespace BeatHeroes.Windows
{
    public class HeroesGalleryWindow : BaseLauncherWindow<HeroesGalleryWindow>
    {
        protected override int Internal_Index => 0;

        protected override void OnShowing()
        {
            base.OnShowing();
            MainWindow.Hide();
        }

        protected override void OnBackButton()
        {
            base.OnBackButton();
            MainWindow.Show();
        }
    }
}