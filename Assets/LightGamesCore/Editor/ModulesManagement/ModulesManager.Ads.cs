using System;
using GameCore.Attributes;
using JetBrains.Annotations;
using UnityEngine;
using static LGCore.Editor.Defines.Names;

namespace LGCore.Editor
{
    internal partial class ModulesManager
    {
        [ColoredField, SerializeField] private AdsModules adsModules = new();

        [Serializable]
        [UsedImplicitly]
        private record AdsModules
        {
            [SerializeField] private ModuleData adMob = ADMOB;
            [SerializeField] private ModuleData cas = CAS;
        }
    }
}