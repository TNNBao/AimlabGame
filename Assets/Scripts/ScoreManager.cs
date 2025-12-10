using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class HighScoreEntry
{
    public string name;
    public float score;
    public string mode;
}

[System.Serializable]
public class HighScores
{
    public List<HighScoreEntry> entryList;
}

public class ScoreManager : MonoBehaviour
{
    // Hàm này gọi khi kết thúc game (EndGame)
    public static void SaveScore(float score, string modeName)
    {
        // 1. Lấy dữ liệu cũ
        string jsonString = PlayerPrefs.GetString("LeaderboardTable", "{}");
        HighScores highScores = JsonUtility.FromJson<HighScores>(jsonString);
        if (highScores == null) highScores = new HighScores() { entryList = new List<HighScoreEntry>() };

        // 2. Thêm điểm mới
        HighScoreEntry newEntry = new HighScoreEntry 
        { 
            name = GlobalData.PlayerName, 
            score = score,
            mode = modeName 
        };
        highScores.entryList.Add(newEntry);

        // 3. Lưu lại (Chưa cần sắp xếp ở đây, lúc nào hiển thị mới sắp xếp)
        string jsonToSave = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString("LeaderboardTable", jsonToSave);
        PlayerPrefs.Save();
    }
    
    // Hàm lấy danh sách ĐÃ SẮP XẾP theo từng Mode
    public static List<HighScoreEntry> GetSortedScores(string modeName)
    {
        string jsonString = PlayerPrefs.GetString("LeaderboardTable", "{}");
        HighScores highScores = JsonUtility.FromJson<HighScores>(jsonString);
        if (highScores == null || highScores.entryList == null) return new List<HighScoreEntry>();

        // Lọc ra danh sách chỉ của Mode này
        var filteredList = highScores.entryList.Where(x => x.mode == modeName).ToList();

        // Logic sắp xếp riêng cho từng Mode
        if (modeName == "BOT_Mode2") 
        {
            // Time Attack: Thời gian THẤP NHẤT lên đầu (Tăng dần)
            return filteredList.OrderBy(x => x.score).Take(10).ToList();
        }
        else
        {
            // Các mode khác: Điểm CAO NHẤT lên đầu (Giảm dần)
            return filteredList.OrderByDescending(x => x.score).Take(10).ToList();
        }
    }
    
    // Xóa dữ liệu (Dùng để reset khi test)
    public static void ClearData()
    {
        PlayerPrefs.DeleteKey("LeaderboardTable");
    }
}