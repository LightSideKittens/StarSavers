using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = FileName, menuName = "Core/StaticMemberReseter/" + FileName, order = 0)]
    public class AssemdefsForStaticReseter : ScriptableObject
    {
        public const string FileName = "AssemdefsForStaticReseter";
        public List<AssemblyDefinitionAsset> assemblies;

        public HashSet<string> assembliesSet
        {
            get
            {
                var set = new HashSet<string>();

                for (int i = 0; i < assemblies.Count; i++)
                {
                    set.Add(assemblies[i].name);
                }
                
                return set;
            }
        }
    }
}