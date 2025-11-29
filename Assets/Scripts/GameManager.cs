using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    [Header("Setup")]
    public GameObject botPrefab;       
    public Collider spawnArea;         
    public GameUI gameUI; 

    [Header("Game Mode Configuration")] 
    public int selectedMode = 1; // Chỉnh tay số 1 hoặc 2 ở đây trong Inspector

    [Header("Mode 1 Settings (Reflex)")]
    public int mode1TotalBots = 30;     // Tổng số bot sẽ xuất hiện
    public float mode1BotDuration = 1.5f; // Thời gian bot tồn tại (giây)

    [Header("Mode 2 Settings (Time Attack)")]
    public int mode2TargetKills = 50;   // Tổng số kill cần đạt được

    [Header("Live Stats (Read Only)")]
    public bool isGameActive = false;
    public float timer = 0f;
    
    public int botsSpawned = 0; // Đếm số bot ĐÃ XUẤT HIỆN
    public int botsKilled = 0;  // Đếm số bot ĐÃ BẮN TRÚNG
    
    public int shotsFired = 0;
    public int shotsHit = 0;

    private GameObject currentBotInstance; // Lưu tham chiếu con bot hiện tại để kiểm soát

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Khi bấm Play, chưa start game ngay, chờ bắn nút Start
        UpdateUI();
    }

    private void Update()
    {
        // Check phím F2 để đổi Mode (Chỉ đổi khi game KHÔNG chạy)
        if (!isGameActive && Keyboard.current != null)
        {
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                // Đảo ngược mode: 1 -> 2, 2 -> 1
                selectedMode = (selectedMode == 1) ? 2 : 1;
                Debug.Log("Đã đổi sang Mode: " + selectedMode);
                UpdateUI();
            }
        }

        if (isGameActive)
        {
            timer += Time.deltaTime;
            if(gameUI != null) gameUI.UpdateTimer(timer);
        }
    }

    // --- LOGIC BẮT ĐẦU GAME ---
    public void StartGame(int modeOverride = 0)
    {
        // Nếu không truyền tham số (modeOverride = 0), dùng mode chỉnh trong Inspector
        int modeToPlay = (modeOverride == 0) ? selectedMode : modeOverride;
        selectedMode = modeToPlay; // Cập nhật lại biến global để các hàm khác biết

        isGameActive = true;
        timer = 0f;
        
        botsSpawned = 0;
        botsKilled = 0;
        shotsFired = 0;
        shotsHit = 0;

        Debug.Log($"Game Started! Mode: {selectedMode}");
        
        UpdateUI(); 
        SpawnBot();
    }

    // --- LOGIC HỦY GAME ---
    public void CancelGame()
    {
        isGameActive = false;
        StopAllCoroutines(); // Dừng việc đếm giờ bot biến mất (Mode 1)

        if (currentBotInstance != null) Destroy(currentBotInstance);

        // Xóa sạch mọi bot còn sót (đề phòng)
        GameObject[] existingBots = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject bot in existingBots) Destroy(bot);

        // Reset về 0
        timer = 0f; botsKilled = 0; botsSpawned = 0; shotsFired = 0; shotsHit = 0;
        
        UpdateUI(); 
        Debug.Log("=== GAME CANCELLED ===");
    }

    public void RestartGame(int mode)
    {
        CancelGame();
        StartGame(mode);
    }

    // --- LOGIC SPAWN BOT ---
    public void SpawnBot()
    {
        if (!isGameActive) return;

        // Kiểm tra điều kiện dừng TRƯỚC khi spawn
        if (CheckEndGameCondition()) return;

        // Sinh vị trí ngẫu nhiên
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.min.y; 

        // Tạo bot mới
        currentBotInstance = Instantiate(botPrefab, new Vector3(x,y,z), Quaternion.Euler(0, 180, 0));
        botsSpawned++; // Tăng biến đếm số lượng bot đã xuất hiện

        BotMovement botMovement = currentBotInstance.GetComponent<BotMovement>();
        if (botMovement != null)
        {
            if (selectedMode == 1)
            {
                // Mode 1: Tắt di chuyển (Bot đứng yên)
                botMovement.isMovingAllowed = false; 
            }
            else
            {
                // Mode 2 (hoặc các mode khác): Bật di chuyển
                botMovement.isMovingAllowed = true; 
            }
        }

        // Nếu là Mode 1: Bắt đầu đếm ngược để bot tự biến mất
        if (selectedMode == 1)
        {
            StartCoroutine(BotLifeCycleRoutine(currentBotInstance));
        }
        
        UpdateUI(); // Cập nhật số lượng (vd: 1/30)
    }

    // Coroutine riêng cho Mode 1: Đợi X giây rồi tự hủy bot
    IEnumerator BotLifeCycleRoutine(GameObject botRef)
    {
        yield return new WaitForSeconds(mode1BotDuration);

        // Sau khi chờ xong, kiểm tra xem con bot đó còn sống không
        if (isGameActive && botRef != null)
        {
            // Nếu còn sống -> Tức là người chơi bắn trượt (Miss)
            Destroy(botRef);
            
            // Spawn con tiếp theo ngay lập tức
            SpawnBot();
        }
    }

    // --- LOGIC XỬ LÝ SỰ KIỆN ---

    public void RegisterShot()
    {
        if (!isGameActive) return;
        shotsFired++;
        UpdateUI(); 
    }

    public void RegisterHit()
    {
        if (!isGameActive) return;
        shotsHit++;
        UpdateUI(); 
    }

    // Hàm này được gọi từ BotHealth khi bot bị bắn chết (HP <= 0)
    public void RegisterKill()
    {
        if (!isGameActive) return;
        
        botsKilled++;
        
        // Nếu là Mode 1: Người chơi bắn trúng trước khi hết giờ -> Hủy coroutine đếm ngược cũ không cần thiết nữa (tự động logic spawn sẽ chạy)
        // Nhưng đơn giản nhất là cứ để SpawnBot gọi tiếp

        SpawnBot(); // Spawn con tiếp theo
    }

    // Kiểm tra xem đã kết thúc game chưa
    private bool CheckEndGameCondition()
    {
        bool isEnded = false;

        if (selectedMode == 1)
        {
            // Mode 1: Dừng khi ĐÃ SPAWN ĐỦ 30 CON (bất kể bắn trúng hay không)
            // Lưu ý: SpawnBot được gọi để spawn con tiếp theo, nên check >= total
            if (botsSpawned >= mode1TotalBots) isEnded = true;
        }
        else if (selectedMode == 2)
        {
            // Mode 2: Dừng khi ĐÃ GIẾT ĐỦ 50 CON
            if (botsKilled >= mode2TargetKills) isEnded = true;
        }

        if (isEnded)
        {
            EndGame();
            return true; // Báo hiệu để không spawn thêm nữa
        }

        return false;
    }

    private void EndGame()
    {
        isGameActive = false;
        StopAllCoroutines();
        
        if (currentBotInstance != null) Destroy(currentBotInstance);

        Debug.Log("=== GAME OVER ===");
        Debug.Log($"Mode: {selectedMode} | Kills: {botsKilled} | Time: {timer:F2}s");
    }

    void UpdateUI()
    {
        if (gameUI == null) return;

        if (!isGameActive)
        {
            gameUI.timerText.text = "00:00";
            // Hiển thị tên Mode hiện tại
            gameUI.scoreText.text = (selectedMode == 1) ? "Mode: Reflex" : "Mode: Time attack";
            gameUI.accuracyText.text = "F2 to Change";
            return;
        }

        float accuracy = (shotsFired > 0) ? (float)shotsHit / shotsFired * 100f : 0;
        
        gameUI.UpdateTimer(timer);
        gameUI.UpdateAccuracy(accuracy);

        // Hiển thị điểm số tùy theo Mode
        if (selectedMode == 1)
        {
            // Mode 1: Hiển thị số Bot đã giết / Tổng số cơ hội (30)
            // Hoặc hiển thị tiến độ: Bot thứ mấy / 30
            gameUI.scoreText.text = $"Score: {botsKilled} | {botsSpawned}";
        }
        else
        {
            // Mode 2: Hiển thị số Bot đã giết / Mục tiêu (50)
            gameUI.UpdateScore(botsKilled, mode2TargetKills);
        }
    }
}