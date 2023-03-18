using System;
using System.Collections.Generic;
using Core.ConfigModule;
using Newtonsoft.Json;

public class TutorialData : JsonBaseConfigData<TutorialData>
{
    public override string FileName { get; set; } = "tutorialData";
    [JsonProperty] private readonly HashSet<Type> completedSteps = new HashSet<Type>();

    public static void OnStepComplete(Type stepType)
    {
        Config.completedSteps.Add(stepType);
    }

    public static bool CheckStepComplete(Type stepType)
    {
        return Config.completedSteps.Contains(stepType);
    }
}
