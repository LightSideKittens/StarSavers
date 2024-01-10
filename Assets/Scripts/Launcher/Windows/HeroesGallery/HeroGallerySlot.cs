using LSCore;
using UnityEngine;

namespace BeatHeroes.Windows
{
    public class HeroGallerySlot : MonoBehaviour
    {
        [Id("Heroes")] 
        [SerializeField] private Id id;
        [SerializeField] private LSButton button;
        [SerializeField] private LSText levelText;
        [SerializeField] private GameObject selectionMark;
        [SerializeField] private Funds price;
        
    }
}