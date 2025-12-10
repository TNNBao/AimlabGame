using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class MainMenuController : MonoBehaviour
{
    // Tên Scene mặc định muốn vào (ví dụ vào phòng bắn Bot trước)
    public string playSceneName = "Game_BOT";
    public TMP_InputField nameInputField; 

    void Start()
    {
        if (PlayerPrefs.HasKey("LastPlayerName"))
        {
            string lastPlayer = PlayerPrefs.GetString("LastPlayerName");
            if(nameInputField != null) nameInputField.text = lastPlayer;
        }
    }

    public void OnPlayButton()
    {
        // 1. Lưu tên vào GlobalData
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            GlobalData.PlayerName = nameInputField.text;
            
            // Lưu xuống máy để lần sau mở game nó tự điền
            PlayerPrefs.SetString("LastPlayerName", GlobalData.PlayerName);
        }
        else
        {
            GlobalData.PlayerName = "Guest";
        }

        // 2. Vào game
        SceneManager.LoadScene(playSceneName);
    }

    public void OnExitButton()
    {
        Debug.Log("Đã thoát game!");
        Application.Quit(); // Chỉ hoạt động khi build ra file .exe
    }

    // Hàm mở hướng dẫn 
    public void OnGuideButton()
    {
        Debug.Log("Hiện bảng hướng dẫn...");
    }

    // Nút Leaderboard
    public void OnLeaderboardButton()
    {
        Debug.Log("Mở bảng xếp hạng...");
    }
}