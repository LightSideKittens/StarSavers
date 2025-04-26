using System.Collections.Generic;
using System.Diagnostics;
using LSCore.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using Unity.Collections;
using Debug = UnityEngine.Debug;

public class PropertyDriver : MonoBehaviour
{
    // ─── исходные данные ─────────────────────────────────────────────
    public SpriteRenderer t1;
    public SpriteRenderer t2;
    public SpriteRenderer t3;

    // ─── созданные массивы ───────────────────────────────────────────
    static List<BoundProperty> floatPropsList = new();
    static List<BoundProperty> discretePropsList = new();
    
    NativeArray<BoundProperty> floatProps;
    NativeArray<BoundProperty> discreteProps;
    
    NativeArray<float> floatValues;
    NativeArray<int> discreteValues;
    
    private static NativeArray<GenericBinding> binding = new(1, Allocator.Persistent);

    [Button]
    void Begin()
    {
        floatPropsList.Clear();
        discretePropsList.Clear();
        
        AddCompProps(t1, "m_Sprite", true);
        /*
        AddCompProps(t2, "m_Color.g", false);
        AddCompProps(t3, "m_Color.b", false);
        */

        floatProps = floatPropsList.ToNativeArray(Allocator.Persistent);
        discreteProps = discretePropsList.ToNativeArray(Allocator.Persistent);
        floatValues = new NativeArray<float>(floatProps.Length, Allocator.Persistent);
        discreteValues = new NativeArray<int>(discreteProps.Length, Allocator.Persistent);

        void AddCompProps(Component tr, string propName, bool isRef)
        {
            var go = tr.gameObject;
            AddProps(tr, go, propName, isRef);
        }

        void AddProps(Object obj, GameObject go, string propName, bool isRef)
        {
            var ok = GenericBindingUtility.CreateGenericBinding(obj, propName, go, isRef, out var d);

            binding[0] = d;
            GenericBindingUtility.BindProperties(
                go,
                binding,
                out var floatPs,
                out var discretePs,
                Allocator.Temp);
            
            floatPropsList.AddRange(floatPs);
            discretePropsList.AddRange(discretePs);
        } 
    }
    

    [Button]
    void Test()
    {
        discreteValues[0] = 42168;
        GenericBindingUtility.SetValues(discreteProps, discreteValues);
    }

    [Button]
    void Stop()
    {
        // 4. Грамотный финал
        GenericBindingUtility.UnbindProperties(floatProps);
        GenericBindingUtility.UnbindProperties(discreteProps);
        floatProps.Dispose();
        discreteProps.Dispose();
    }
}
