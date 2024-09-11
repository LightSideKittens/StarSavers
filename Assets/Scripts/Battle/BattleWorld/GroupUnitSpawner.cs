using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using LSCore.Extensions.Unity;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif

namespace LSCore.BattleModule
{
    public class GroupUnitSpawner : MonoBehaviour
    {
        [Serializable]
        internal struct LeaderData
        {
            [Required]
            [ValueDropdown("Ids")] public Id id;
            public Vector2 position;

#if UNITY_EDITOR
            public IEnumerable<Id> Ids => currentInspected?.levelsManager.Ids;
#endif
        }
        
        [Serializable]
        internal struct Data
        {
            [ValueDropdown("Ids")] public Id id;
            public Vector2[] positions;

#if UNITY_EDITOR
            public IEnumerable<Id> Ids => currentInspected?.levelsManager.Ids;
#endif
        }

#if UNITY_EDITOR
        [SerializeField] private LevelsManager levelsManager;
        private static GroupUnitSpawner currentInspected;
        
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            OnValidate();
        }
        
        [OnInspectorGUI]
        private void OnInspectorGui()
        {
            currentInspected = this;
        }

        [OnInspectorDispose]
        private void OnInspectorDisable()
        {
            currentInspected = null;
            DestroyAllUnits();
        }

        private void DestroyAllUnits()
        {
            if (this == null) return;

            Unit[] toDestroy = Resources.FindObjectsOfTypeAll<Unit>();
            
            for (int i = 0; i < toDestroy.Length; i++)
            {
                var go = toDestroy[i].gameObject;
                if (go.hideFlags == HideFlags.HideAndDontSave)
                {
                    DestroyImmediate(go);
                }
            }
        }
        
        internal void OnValidate()
        {
            if(World.IsPlaying) return;
            if(EditorUtility.IsPersistent(gameObject)) return;
            if(!Selection.objects.Contains(gameObject)) return;
            
            EditorApplication.update += Do;
            
            return;
            void Do()
            {
                EditorApplication.update -= Do;
                
                DestroyAllUnits();
                if(levelsManager == null) return;
                if(leader.id == null) return;
                
                var unit = Instantiate(levelsManager.GetLevel<Unit>(leader.id, 1), transform);
                unit.gameObject.hideFlags = HideFlags.HideAndDontSave;
                unit.GetComponent<Transform>().localPosition = leader.position;

                for (int i = 0; i < units.Length; i++)
                {
                    var d = units[i];
                    if (d.id == null)
                    {
                        continue;
                    }

                    for (int j = 0; j < d.positions.Length; j++)
                    {
                        unit = Instantiate(levelsManager.GetLevel<Unit>(d.id, 1), transform);
                        unit.gameObject.hideFlags = HideFlags.HideAndDontSave;
                        unit.GetComponent<Transform>().localPosition = d.positions[j];
                    }
                }
            }
        }
#endif

        [SerializeField] [BoxGroup] internal LeaderData leader;
        [PropertySpace(SpaceBefore = 10)]
        [SerializeField] internal Data[] units;
        
        public void Spawn()
        {
            Vector2 position = BattleWorld.CameraRect.RandomPointAroundRect(2);
            
            var unit = OpponentWorld.Spawn(leader.id);
            var moveComp = unit.GetComp<MoveComp>();
            var direction = ((Vector2)moveComp.Target.position - position).normalized;
            float degrees = Vector2.SignedAngle(Vector2.up, direction);
            var targetPos = position + leader.position;
            targetPos = targetPos.RotateAroundPoint(position, degrees);
            unit.transform.position = targetPos;

            for (int i = 0; i < units.Length; i++)
            {
                var d = units[i];
                
                for (int j = 0; j < d.positions.Length; j++)
                {
                    unit = OpponentWorld.Spawn(d.id);
                    targetPos = position + d.positions[j];
                    targetPos = targetPos.RotateAroundPoint(position, degrees);
                    unit.transform.position = targetPos;
                }
            }
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(GroupUnitSpawner))]
    public class GroupUnitSpawnerEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            GroupUnitSpawner myComponent = (GroupUnitSpawner)target;

            var data = myComponent.units;
            if (data == null) return;

            var position = myComponent.leader.position;
            var offset = (Vector2)myComponent.transform.position;
                
            var newPosition = (Vector2)Handles.PositionHandle(position + offset, Quaternion.identity) - offset;
                
            if (newPosition != position)
            {
                Undo.RecordObject(myComponent, $"Move Unit Leader");
                myComponent.leader.position = newPosition;
                myComponent.OnValidate();
                EditorUtility.SetDirty(myComponent);
            }

            for (int i = 0; i < data.Length; i++)
            {
                ref var d = ref data[i];
                
                for (int j = 0; j < d.positions.Length; j++)
                {
                    position = d.positions[j];
                    offset = myComponent.transform.position;
                
                    newPosition = (Vector2)Handles.PositionHandle(position + offset, Quaternion.identity) - offset;
                
                    if (newPosition != position)
                    {
                        Undo.RecordObject(myComponent, $"Move Unit at ({i}, {j}) indexes");
                        d.positions[j] = newPosition;
                        myComponent.OnValidate();
                        EditorUtility.SetDirty(myComponent);
                    }
                }

            }
        }
    }
#endif
}