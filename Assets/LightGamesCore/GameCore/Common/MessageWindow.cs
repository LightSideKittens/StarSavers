using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : BaseWindow<MessageWindow>
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;

    public static Button Button => Instance.button;

    protected override void Init()
    {
        base.Init();
        button.gameObject.SetActive(false);
    }

    public static void Show(string message)
    {
        Show();
        Instance.text.text = message;
    }
}
