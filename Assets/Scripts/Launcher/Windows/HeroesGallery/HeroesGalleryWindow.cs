using UnityEngine;

namespace BeatHeroes.Windows
{
    public class HeroesGalleryWindow : BaseLauncherWindow<HeroesGalleryWindow>
    {
        [SerializeField] private HeroGallerySlot[] slots;
        [SerializeField] private Transform unlocked;
        [SerializeField] private Transform locked;
        
        protected override int Internal_Index => 0;

        protected override void Init()
        {
            base.Init();

            for (int i = 0; i < slots.Length; i++)
            {
                Transform parent = unlocked;
                var slot = slots[i];
                
                if (slot.IsBlocked(out _))
                {
                    parent = locked;
                }
                
                Instantiate(slot, parent);
            }
        }

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