using UnityEngine;

public class BotHealth : MonoBehaviour
{
    [Header("Bot Stats")]
    [SerializeField] float maxHealth = 150f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Bot nhận {damageAmount} damage. HP còn lại: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Sau này sẽ thêm logic spawn bot mới ở đây
        Debug.Log("Bot đã bị tiêu diệt!");
        Destroy(gameObject);
    }
}
