using UnityEngine;
using UnityEngine.SceneManagement; // Để chuyển scene

public class MainMenuController : MonoBehaviour
{
    // Tên Scene mặc định muốn vào (ví dụ vào phòng bắn Bot trước)
    public string playSceneName = "Game_BOT"; 

    public void OnPlayButton()
    {
        // Load Scene chơi game
        SceneManager.LoadScene(playSceneName);
    }

    public void OnExitButton()
    {
        Debug.Log("Đã thoát game!");
        Application.Quit(); // Chỉ hoạt động khi build ra file .exe
    }

    // Hàm mở hướng dẫn (tạm thời chưa làm gì)
    public void OnGuideButton()
    {
        Debug.Log("Hiện bảng hướng dẫn...");
    }
}