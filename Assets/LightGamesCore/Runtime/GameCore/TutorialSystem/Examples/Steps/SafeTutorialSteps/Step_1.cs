using System;
using DG.Tweening;
using UnityEngine;

namespace TutorialSystem.Examples.Steps.SafeTutorialSteps
{
    public class Step_1 : BaseTutorialStep
    {
        public override void StartTutorial(Action continuation)
        {
            MessageWindow.Show($"Tutorial step {GetType().Name} is running...");
            new GameObject(GetType().Name).transform.DOMove(Vector3.up, 5).OnComplete(() =>
            {
                MessageWindow.Hide();
                new CountDownTimer(2, true).Stopped += continuation.Invoke;
            });
        }
    }
}
