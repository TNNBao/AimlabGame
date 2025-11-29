using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public enum ButtonType { StartGame, CancelSession } 
    
    [Header("Cấu hình Nút")]
    public ButtonType buttonType;

    
    public void OnHit()
    {
        // Hiệu ứng nút nảy lên 1 chút khi bắn (Visual feedback)
        transform.localScale *= 0.9f; 
        Invoke("ResetSize", 0.1f);

        if (buttonType == ButtonType.StartGame)
        {
            if (GameManager.Instance != null)
            {
                // Gửi số gameModeToStart (1 hoặc 2) sang GameManager
                GameManager.Instance.StartGame();
            }
        }
        else if (buttonType == ButtonType.CancelSession)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CancelGame();
            }
        }
    }

    void ResetSize()
    {
        transform.localScale /= 0.9f;
    }
}