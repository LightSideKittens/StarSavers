using System;
using Core.ConditionModule;

[Serializable]
public class TestButtonCondition_1 : BaseCondition
{
    protected override bool IsCompleted { get; set; }
    protected override void Init()
    {
        TestConditionSystem.Button1Clicked += OnClick;
    }

    public int maxValue = 0;
    public string name;
    public TestButtonCondition_2 condition2;

    private int count;

    private void OnClick()
    {
        count++;
        
        if (count >= maxValue)
        {
            IsCompleted = true;
            TestConditionSystem.Button1Clicked -= OnClick;
        }
    }
}

[Serializable]
public class Not<T> : BaseCondition where T : BaseCondition
{
    protected override bool IsCompleted
    {
        get => !condition;
        set { }
    }

    public T condition;
}