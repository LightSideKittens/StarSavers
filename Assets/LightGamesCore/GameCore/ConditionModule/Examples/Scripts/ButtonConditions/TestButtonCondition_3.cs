using System;
using Core.ConditionModule;
using Core.Extensions.Unity;

[Serializable]
public class TestButtonCondition_3 : BaseCondition
{
    protected override bool IsCompleted { get; set; }

    protected override void Init()
    {
        TestConditionSystem.Button3.AddListener(OnClick);
    }

    private void OnClick()
    {
        IsCompleted = true;
        TestConditionSystem.Button3.RemoveListener(OnClick);
    }
}
