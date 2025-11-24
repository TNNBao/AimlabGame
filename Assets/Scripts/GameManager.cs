using UnityEngine;
using UnityEngine.UI; // Nếu sau này dùng UI
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton để gọi từ bất cứ đâu

    [Header("Settings")]
    public GameObject botPrefab;       // Kéo Prefab Bot vào đây
    public Collider spawnArea;         // Kéo BoxCollider SpawnArea vào đây
    
    [Header("Game Mode Settings")]
    public int mode1TargetKills = 30;  // GDD: Mode 1 - 30 bots
    public int mode2TargetKills = 50;  // GDD: Mode 2 - 50 bots
    public int currentTargetKills;     // Số kill mục tiêu của mode hiện tại

    [Header("Live Stats")]
    public bool isGameActive = false;
    public float timer = 0f;
    public int botsKilled = 0;
    public int shotsFired = 0;
    public int shotsHit = 0;

    private void Awake()
    {
        // Tạo Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Tạm thời test: Tự động start game khi chạy Scene (sau này sẽ bắn nút Start để gọi hàm này)
        StartGame(1); 
    }

    private void Update()
    {
        if (isGameActive)
        {
            timer += Time.deltaTime;
        }
    }

    // Hàm bắt đầu game, nhận vào mode (1 hoặc 2)
    public void StartGame(int mode)
    {
        isGameActive = true;
        timer = 0f;
        botsKilled = 0;
        shotsFired = 0;
        shotsHit = 0;

        // Setup theo GDD
        if (mode == 1) currentTargetKills = mode1TargetKills;
        else currentTargetKills = mode2TargetKills;

        Debug.Log("Game Started! Mode: " + mode);
        
        // Spawn con bot đầu tiên
        SpawnBot();
    }

    // Hàm sinh Bot ngẫu nhiên trong vùng SpawnArea
    public void SpawnBot()
    {
        if (!isGameActive) return;

        // Tính toán vị trí ngẫu nhiên trong Box Collider
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.min.y; // Spawn tại mặt sàn của box

        Vector3 spawnPos = new Vector3(x, y, z);
        Quaternion spawnRot = Quaternion.Euler(0, 180, 0); // Xoay bot mặt về phía người chơi (tùy chỉnh trục Y)

        Instantiate(botPrefab, spawnPos, spawnRot);
    }

    // Hàm gọi khi người chơi BẮN (gắn vào súng)
    public void RegisterShot()
    {
        if (!isGameActive) return;
        shotsFired++;
    }

    // Hàm gọi khi bắn TRÚNG (gắn vào BotHitbox hoặc súng)
    public void RegisterHit()
    {
        if (!isGameActive) return;
        shotsHit++;
    }

    // Hàm gọi khi Bot CHẾT (gắn vào BotHealth)
    public void RegisterKill()
    {
        if (!isGameActive) return;
        
        botsKilled++;
        Debug.Log($"Kill: {botsKilled}/{currentTargetKills}");

        // Kiểm tra điều kiện thắng (GDD: Đủ số bot thì dừng)
        if (botsKilled >= currentTargetKills)
        {
            EndGame();
        }
        else
        {
            // Nếu chưa xong thì spawn con tiếp theo
            SpawnBot();
        }
    }

    private void EndGame()
    {
        isGameActive = false;
        
        // Tính Accuracy
        float accuracy = 0;
        if (shotsFired > 0) 
            accuracy = (float)shotsHit / shotsFired * 100f;

        Debug.Log("=== GAME OVER ===");
        Debug.Log($"Time: {timer.ToString("F2")}s");
        Debug.Log($"Accuracy: {accuracy.ToString("F1")}% ({shotsHit}/{shotsFired})");
        
        // TODO: Hiện bảng điểm (Scoreboard) ở đây
    }
}