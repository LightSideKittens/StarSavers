using System;
using Core.Extensions.Unity;
using DG.Tweening;
using UnityEngine;

namespace TutorialSystem.Examples.Steps.SafeTutorialSteps
{
    public class Step_3 : BaseTutorialStep
    {
        public override void StartTutorial(Action continuation)
        {
            MessageWindow.Show($"Tutorial step {GetType().Name} is running...");
            continuation += MessageWindow.Hide;
            new GameObject(GetType().Name).transform.DOMove(Vector3.back, 7).OnComplete(() =>
            {
                MessageWindow.Button.gameObject.SetActive(true);
                MessageWindow.Button.AddListener(continuation.Invoke);
            });
        }
    }
}
