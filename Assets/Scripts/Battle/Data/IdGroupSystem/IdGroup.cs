using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class IdGroup : SerializedScriptableObject
{
    [OdinSerialize] [HideReferenceObjectPicker] 
    [ValueDropdown("Ids", IsUniqueList = true)]
    private HashSet<Id> ids = new();

    public HashSet<Id> AllIds => ids;
    
#if UNITY_EDITOR
    private IEnumerable<Id> Ids
    {
        get
        {
            var path = this.GetFolderPath();
            var allIds = AssetDatabaseUtils.LoadAllAssets<Id>(paths: path);
            
            foreach (var id in allIds)
            {
                yield return id;
            }
        }
    }
#endif
}