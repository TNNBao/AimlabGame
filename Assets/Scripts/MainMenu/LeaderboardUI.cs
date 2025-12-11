using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentContainer; // Kéo object "Content" trong ScrollView vào đây
    public GameObject rowPrefab;       // Kéo Prefab "ScoreRowPrefab" vào đây

    [Header("Mode Buttons")] // Kéo 4 nút Tab vào đây để đổi màu khi chọn
    public Button btnReflex;
    public Button btnTimeAttack;
    public Button btnGridshot;
    public Button btnTracking;

    private void Start()
    {
        // Mặc định mở tab đầu tiên
        ShowReflex();
    }

    // Các hàm này gắn vào 4 nút Tab
    public void ShowReflex()     { LoadLeaderboard("BOT_Mode1"); HighlightTab(btnReflex); }
    public void ShowTimeAttack() { LoadLeaderboard("BOT_Mode2"); HighlightTab(btnTimeAttack); }
    public void ShowGridshot()   { LoadLeaderboard("DOT_Mode1"); HighlightTab(btnGridshot); }
    public void ShowTracking()   { LoadLeaderboard("DOT_Mode2"); HighlightTab(btnTracking); }

    void LoadLeaderboard(string modeName)
    {
        // 1. Xóa danh sách cũ
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Lấy danh sách điểm mới
        List<HighScoreEntry> scores = ScoreManager.GetSortedScores(modeName);

        // 3. Tạo dòng mới (Spawn Row)
        for (int i = 0; i < scores.Count; i++)
        {
            GameObject newRow = Instantiate(rowPrefab, contentContainer);
            
            // Tìm các Text trong Row (Giả sử thứ tự con: 0=Rank, 1=Name, 2=Score)
            // Cách an toàn hơn là tạo script riêng cho Row, nhưng tìm theo index cho nhanh:
            TextMeshProUGUI[] texts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            
            if (texts.Length >= 3)
            {
                texts[0].text = $"#{i + 1}"; // Hạng
                texts[1].text = scores[i].name; // Tên

                // Hiển thị Điểm hoặc Thời gian tùy mode
                if (modeName == "BOT_Mode2")
                {
                    // Format giây thành 00:00.00
                    float t = scores[i].score;
                    System.TimeSpan ts = System.TimeSpan.FromSeconds(t);
                    texts[2].text = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                }
                else
                {
                    // Điểm số bình thường
                    texts[2].text = scores[i].score.ToString("0");
                }
            }
        }
    }

    void HighlightTab(Button selectedBtn)
    {
        // Reset màu tất cả nút về trắng (hoặc xám)
        btnReflex.interactable = true;
        btnTimeAttack.interactable = true;
        btnGridshot.interactable = true;
        btnTracking.interactable = true;

        // Làm mờ nút đang chọn (để biết đang ở tab nào)
        selectedBtn.interactable = false; 
    }
    
    // Nút Reset data (nếu cần test)
    public void OnResetData()
    {
        ScoreManager.ClearData();
        ShowReflex(); // Reload lại
    }
}