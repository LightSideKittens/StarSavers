using Core.ConditionModule;
using Core.ConfigModule;
using Core.ConfigModule.Attributes;
using Newtonsoft.Json;
using UnityEngine;

[Generatable]
public class GameEventConditionsData : JsonBaseConfigData<GameEventConditionsData>
{
    [JsonProperty] public ConditionsContainer StartCondition { get; private set; }
    [JsonProperty] public ConditionsContainer EndCondition { get; private set; }

    protected override void OnLoading()
    {
        base.OnLoading();
        Settings.TypeNameHandling = TypeNameHandling.Auto;
    }

    protected override void SetDefault()
    {
        base.SetDefault();

        var test1 = new TestButtonCondition_1()
        {
            name = "TestNAME",
            maxValue = 1,
            condition2 = new TestButtonCondition_2()
        };

        StartCondition = ConditionsBuilder
            .If(test1)
            .And<TestButtonCondition_4>()
            .Or<TestButtonCondition_4>()
            .Or<TestButtonCondition_4>()
            .Or<TestButtonCondition_4>()
            .And<TestButtonCondition_4>()
            .Or<TestButtonCondition_4>()
            .Or<TestButtonCondition_4>()
            .Or<TestButtonCondition_4>()
            .Or<TestButtonCondition_4>()
            ;
        
        EndCondition = ConditionsBuilder
            .If<TestButtonCondition_2>()
            .And<TestButtonCondition_3>()
            ;
    }
}
