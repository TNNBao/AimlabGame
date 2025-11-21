using UnityEngine;

public class BotHitbox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public BotHealth botHealthRef; 
    
    [Tooltip("x3 for Head, x1 for Body, x0.6 for Legs")]
    public float damageMultiplier = 1f;

    public void OnHit(float baseDamage)
    {
        if (botHealthRef != null)
        {
            float finalDamage = baseDamage * damageMultiplier;
            botHealthRef.TakeDamage(finalDamage);
        }
    }
}
