using System;
using System.Reflection;
//using HarmonyLib;
using UnityEditor;
using UnityEngine;
using static WindowsExtender;

public class BaseWindowExtender
{
    protected virtual Type WindowType => typeof(WindowsExtender);
    public virtual void OnPreGUI(){}
    public virtual void OnPostGUI(){}
    protected Rect Rect => GUIUtility.ScreenToGUIRect(window.position);
    protected Type windowType;
    protected EditorWindow window;

    public BaseWindowExtender()
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        windowType = WindowType;
        var originalOnGui = windowType.GetMethod("OnGUI", flags);
        Extenders.Add(GetType(), this);
        var prefix = OnPreGUIMethod.MakeGenericMethod(GetType());
        var postfix = OnPostGUIMethod.MakeGenericMethod(GetType());
        //WindowsExtender.Harmony.Patch(originalOnGui, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
    }

    public void SetWindowIfNull()
    {
        if (window == null) window = EditorWindow.GetWindow(windowType);
    }
}
