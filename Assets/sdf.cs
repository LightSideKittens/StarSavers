using Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class sdf : MonoBehaviour
{
    [SerializeField] private ObjectProvider<Image> back;
    private Image backImage;

    private InAction<MonoBehaviour> d;

    private void D(in MonoBehaviour d)
    {
        
    }

    private void Awake()
    {
        d = D;

        backImage = back;
        Debug.Log($"[Malvis] {((Image)back).color}");;

        new CountDownTimer(1, true, true).Elapsed += () => Debug.Log("SS");
        new CountDownTimer(2, true, true).Elapsed += () => Debug.Log("SS2123");
        new CountDownTimer(5, true).Stopped += () => Debug.Log("dsfdsf");

        new CanvasGroup().DOFade(1, 0);
    }
}
