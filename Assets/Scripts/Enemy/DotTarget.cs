using UnityEngine;

public class DotTarget : MonoBehaviour
{
    public void OnHit()
    {
        int currentMode = 1;
        if (GameManager.Instance != null) 
            currentMode = GameManager.Instance.selectedMode;

        if (currentMode == 1)
        {
            // --- MODE 1: GRIDSHOT (Bắn là nổ) ---
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterKill(); 
            }
            Destroy(gameObject); 
        }
        else
        {
            // --- MODE 2: TRACKING  ---
            transform.localScale = Vector3.one * 1.2f;
            Invoke("ResetScale", 0.1f);
        }
    }

    void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
}