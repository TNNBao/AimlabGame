using UnityEngine;
using TMPro; // Bắt buộc phải có để dùng TextMeshPro

public class GameUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI accuracyText;

    // Cập nhật đồng hồ
    public void UpdateTimer(float timeInSeconds)
    {
        // Chuyển đổi giây thành định dạng 00:00
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100) % 100);

        // Hiển thị dạng Phút:Giây:Mili (hoặc bỏ Mili nếu muốn gọn)
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); 
    }

    // Cập nhật điểm (Số kill / Mục tiêu)
    public void UpdateScore(int currentKills, int targetKills)
    {
        scoreText.text = $"Score: {currentKills} / {targetKills}";
    }

    // Cập nhật độ chính xác
    public void UpdateAccuracy(float accuracy)
    {
        accuracyText.text = $"Accuracy: {accuracy:F1}%"; // F1 là lấy 1 số thập phân
    }
}