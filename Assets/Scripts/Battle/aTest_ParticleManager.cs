using UnityEngine;
using UnityEngine.UIElements;

public class aTest_ParticleManager : MonoBehaviour
{
    
    public ParticleSystem hitEffect;
    public ParticleSystem smokeEffect;
    public ParticleSystem[] Claws;
    public ParticleSystem swordWave;
    public ParticleSystem sphereEffect;
    public ParticleSystem enchantEffect;
    
    public ParticleSystem[] EnchantPortalKunai;

    
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
    public void PlaySphereEffect() => sphereEffect.Play();

    public void PlaySwordWaveEffect() => swordWave.Play();
    public void PlayPortalEnchantEffect() => enchantEffect.Play();

    public void PlayEnchantPortalsEffect()
    {
        EnchantPortalKunai[Random.Range(0, EnchantPortalKunai.Length)].Play();
    }

}