using TutorialSystem.Examples.Steps.SafeTutorialSteps;
using UnityEngine;

public class TutorialStepTest : MonoBehaviour
{
    private void Start()
    {
        TryOpenSafe();
    }

    private void TryOpenSafe()
    {
        Debug.Log("Step 1 Before Tutorial");
        
        TutorialManager.WaitStepFinish<Step_1>(() => { 
        Debug.Log("Step 1 After Tutorial");
        TryCloseSafe(); });
    }
    
    private void TryCloseSafe()
    {
        Debug.Log("Step 2 Before Tutorial");
        
        TutorialManager.WaitStepFinish<Step_2>(() =>
        {
            Debug.Log("Step 2 After Tutorial");
            CompleteTutorialByButton();
        });
    }

    private void CompleteTutorialByButton()
    {
        Debug.Log("Step 3 Before Tutorial");
        TutorialManager.WaitStepFinish<Step_3>(() => { 
        Debug.Log("Step 3 After Tutorial"); });
    }
}
