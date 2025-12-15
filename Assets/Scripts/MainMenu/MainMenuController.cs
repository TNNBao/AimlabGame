using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class MainMenuController : MonoBehaviour
{
    // Tên Scene mặc định muốn vào (ví dụ vào phòng bắn Bot trước)
    public string playSceneName = "Game_BOT";
    public GameObject leaderboardPanel;
    public GameObject guidePanel;
    public TMP_InputField nameInputField; 

    void Start()
    {
        // PlayerPrefs.DeleteAll();
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
        guidePanel.SetActive(true);
    }

    // Nút Leaderboard
    public void OnLeaderboardButton()
    {
        leaderboardPanel.SetActive(true);
    }
}