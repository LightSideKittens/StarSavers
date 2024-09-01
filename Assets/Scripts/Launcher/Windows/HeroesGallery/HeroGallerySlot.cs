using Battle.Data;
using StarSavers.Data;
using LSCore;
using LSCore.Extensions;
using LSCore.LevelSystem;
using UnityEngine;

namespace StarSavers.Windows
{
    public class HeroGallerySlot : MonoBehaviour
    {
        [Id("Heroes")] 
        [SerializeField] private Id id;
        [SerializeField] private LSButton button;
        [SerializeField] private LSText levelText;
        [SerializeField] private CanvasRenderer selectionMark;
        [SerializeField] private LSSlider rankSlider;
        [SerializeField] private Funds price;
        
        public bool IsBlocked(out int level) => !UnlockedLevels.TryGetLevel(id, out level);
        
        private void Awake()
        {
            button.Clicked += OnButton;
            
            if (!IsBlocked(out var level) && HeroRankIconsConfigs.ById.TryGetValue(id, out var icons))
            {
                rankSlider.gameObject.SetActive(true);
                icons.Load().Icons.TryGet(id, out var data);
                var (sprite, rank, intervalData) = data;
                rankSlider.Icon.sprite = sprite;
                rankSlider.value = rank;
                rankSlider.maxValue = intervalData.to;
            }
            else
            {
                rankSlider.gameObject.SetActive(false);
            }
            
            levelText.text = $"{level}";
            selectionMark.SetAlpha(PlayerData.IsSelected(id).ToInt());
        }

        private void OnButton()
        {
            HeroWindow.Show();
        }
    }
}