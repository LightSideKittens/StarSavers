using System;
using Core.ConditionModule;
using Core.Extensions.Unity;

[Serializable]
public class TestButtonCondition_4 : BaseCondition
{
    protected override bool IsCompleted { get; set; }
    protected override void Init()
    {
        TestConditionSystem.Button4.AddListener(OnClick);
    }

    private void OnClick()
    {
        IsCompleted = true;
        TestConditionSystem.Button4.RemoveListener(OnClick);
    }
}
