using System;
using DG.Tweening;
using UnityEngine;

namespace TutorialSystem.Examples.Steps.SafeTutorialSteps
{
    public class Step_2 : BaseTutorialStep
    {
        public override void StartTutorial(Action continuation)
        {
            MessageWindow.Show($"Tutorial step {GetType().Name} is running...");
            new GameObject(GetType().Name).transform.DOMove(Vector3.back, 5).OnComplete(() => OnComplete(continuation));
        }

        private static void OnComplete(Action continuation)
        {
            MessageWindow.Hide();
            continuation.Invoke();
        }
    }
}
