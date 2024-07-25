using UnityEngine;

public class aTest_ParticleManager : MonoBehaviour
{
    public ParticleSystem hitEffect;
    public ParticleSystem smokeEffect;
    public ParticleSystem[] Claws;
    
    private int currentIndex = 0;
    
    public void PlayHitEffect() => hitEffect.Play();
    public void PlaySmokeEffect() => smokeEffect.Play();
    public void PlayClawsEffect()
    {
        if (currentIndex < Claws.Length)
        {
            Claws[currentIndex].Play();
            currentIndex++;
        }
    }
    public void ResetIndex()
    {
        currentIndex = 0;
    }
}