using GameCore.Battle.Data;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class HeroView
    {
        public static HeroView PrevSelectedHero;
        private Toggle toggle;
        private int id;

        public Toggle Toggle => toggle;
        
        public void Init(Toggle toggle, int name, Sprite image)
        {
            this.toggle = toggle;
            id = name;
            toggle.targetGraphic.GetComponent<Image>().sprite = image;
            toggle.onValueChanged.AddListener(OnSelectedHeroChanged);
            SetSelected(false);
        }

        public void SetSelected(bool value)
        {
            toggle.isOn = value;
            if (value)
            {
                PrevSelectedHero = this;
            }
        }
        
        private void OnSelectedHeroChanged(bool value)
        {
            if (value)
            {
                if (PrevSelectedHero != null)
                {
                    PrevSelectedHero.Toggle.isOn = false;
                }
                PrevSelectedHero = this;
                PlayerData.Config.SelectedHero = id;
            }
        }
    }
}