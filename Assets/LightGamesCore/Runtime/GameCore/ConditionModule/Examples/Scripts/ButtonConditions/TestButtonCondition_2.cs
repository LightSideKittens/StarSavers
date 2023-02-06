using System;
using Core.ConditionModule;

[Serializable]
public class TestButtonCondition_2 : BaseCondition
{
    protected override bool IsCompleted
    {
        get => TestConditionSystem.isButton2Clicked;
        set { }
    }
}
