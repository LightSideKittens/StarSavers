using UnityEngine;

public class aTest_ParticleManager : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public GameObject smokeEffectPrefab;
    public GameObject asdf;
    
    public Transform hitEffectPosition;
    public Transform smokeEffectPosition;

    public float particleSize = 2f;

    public void PlayHitEffect()
    {
        if (hitEffectPrefab != null && hitEffectPosition != null)
        {
            GameObject hitEffectInstance = Instantiate(hitEffectPrefab, 
                hitEffectPosition.position, Quaternion.identity);
            Destroy(hitEffectInstance, 1.0f);
        }
        else
        {
            Debug.LogWarning("Hit Effect Prefab or Hit Effect Position is not assigned!");
        }
    }
    
    public void PlaySmokeEffect()
    {
        if (smokeEffectPrefab != null && smokeEffectPosition != null)
        {
            GameObject smokeEffectInstance = Instantiate(smokeEffectPrefab, 
                smokeEffectPosition.position, Quaternion.identity);
            
            // Увеличиваем масштаб частиц дыма в 3 раза
            smokeEffectInstance.transform.localScale = 
                new Vector3(particleSize, particleSize, particleSize);
            HideObject();
            Destroy(smokeEffectInstance, 2.0f);
        }
        else
        {
            Debug.LogWarning("Smoke Effect Prefab or Smoke Effect Position is not assigned!");
        }
    }
    
    public void HideObject()
    {
        gameObject.SetActive(false);
    }
}