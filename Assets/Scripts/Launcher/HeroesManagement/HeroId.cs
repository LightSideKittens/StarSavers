using LSCore;
using LSCore.LevelSystem;
using UnityEngine;

namespace Launcher.HeroesManagement
{
    public class HeroId : MonoBehaviour
    {
        [Id(typeof(LevelIdGroup), "Heroes")] 
        public Id id;
    }
}