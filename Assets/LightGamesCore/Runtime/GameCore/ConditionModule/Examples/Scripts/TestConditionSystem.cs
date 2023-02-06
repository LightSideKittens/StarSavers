using System;
using Core.ConfigModule;
using Core.Extensions.Unity;
using Core.SingleService;
using UnityEngine;
using UnityEngine.UI;

public class TestConditionSystem : SingleService<TestConditionSystem>
{
    public static event Action Button1Clicked;
    public static bool isButton2Clicked;

    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;

    public static Button Button3 => Instance.button3;
    public static Button Button4 => Instance.button4;
    private Timer.Base timer;

    public static void Initialize()
    {
        var instance = Instance;
        instance.button1.AddListener(() => Button1Clicked?.Invoke());
        instance.button2.AddListener(() => isButton2Clicked = true);
        instance.button2.targetGraphic.color = Color.black;
        instance.button3.targetGraphic.color = Color.black;
        Instance.gameObject.SetActive(false);
    }

    protected override void Init()
    {
        base.Init();
        MessageWindow.Show("Wait for downloading config...");
        
        RemoteBaseConfigsVersionsData.Fetch(() =>
        {
            GameEventConditionsData.Fetch(() =>
            {
                MessageWindow.Hide();
                Instance.gameObject.SetActive(true);
                timer = new CountDownTimer(1, true, true);
                timer.Elapsed += Check;
            });
            GameEventConditionsData.Fetch(() => Debug.Log("[Malvis] GameEventConditionsData.Fetch1"));
            GameEventConditionsData.Fetch(() => Debug.Log("[Malvis] GameEventConditionsData.Fetch2"));
            GameEventConditionsData.Fetch(() => Debug.Log("[Malvis] GameEventConditionsData.Fetch3"));
            GameEventConditionsData.Fetch(() => Debug.Log("[Malvis] GameEventConditionsData.Fetch4"));
            GameEventConditionsData.Fetch(() => Debug.Log("[Malvis] GameEventConditionsData.Fetch5"));
            GameEventConditionsData.Fetch(() => Debug.Log("[Malvis] GameEventConditionsData.Fetch6"));
        });

        RemoteBaseConfigsVersionsData.Fetch(() =>
        {
            Debug.Log($"[Malvis] {RemoteBaseConfigsVersionsData.CompareRemoteToLocal("")}");
        });
    }

    private void Check()
    {
        if (GameEventConditionsData.Config.StartCondition)
        {
            timer.Elapsed -= Check;
            timer.Elapsed += CheckFinishGameEvent;
            button2.targetGraphic.color = Color.yellow;
            button3.targetGraphic.color = Color.yellow;
            CheckFinishGameEvent();
        }
    }

    private void CheckFinishGameEvent()
    {
        if (GameEventConditionsData.Config.EndCondition)
        {
            button1.targetGraphic.color = Color.green;
            button2.targetGraphic.color = Color.green;
            button3.targetGraphic.color = Color.green;
            button4.targetGraphic.color = Color.green;
            timer.Elapsed -= CheckFinishGameEvent;
        }
    }
}
