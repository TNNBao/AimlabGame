using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using StarterAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("--- GENERAL SETUP ---")]
    public bool isDotScene = false; 
    public Collider spawnArea;
    public GameUI gameUI;
    [Range(1, 2)] public int selectedMode = 1;

    [Header("--- BOT SETTINGS ---")]
    public GameObject botPrefab;
    public int botMode1TotalSpawns = 30; // Mode 1: Giới hạn số lần xuất hiện
    public float botMode1Duration = 1.5f; // Thời gian tồn tại của bot Mode 1
    public int botMode2TargetKills = 50;  // Mode 2: Giới hạn số kill

    [Header("--- DOT SETTINGS ---")]
    public GameObject dotPrefab; 
    public int dotMode1Concurrent = 3; 
    public float dotMode1Duration = 60f;
    public float dotMode2Duration = 30f;

    [Header("--- UI REFERENCES ---")]
    public GameObject pausePanel; // Kéo cái Panel vừa tạo vào đây
    public UnityEngine.UI.Slider sensitivitySlider; // Kéo cái thanh trượt vào đây
    public StarterAssets.FirstPersonController playerController; // Kéo nhân vật vào để chỉnh tốc độ chuột
    public StarterAssetsInputs starterAssetsInputs;
    
    // Biến thống kê
    public bool isPaused = false; 
    public bool isGameActive = false;
    public bool isGameOver = false; // [MỚI] Trạng thái kết thúc để hiện điểm
    
    public float timer = 0f;
    public int botsKilled = 0; 
    public int botsSpawned = 0; // Đếm số lượng đã sinh ra (Quan trọng cho Mode 1)
    
    public int shotsFired = 0;
    public int shotsHit = 0;

    private int currentActiveTargets = 0; // Số lượng mục tiêu đang sống

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() 
    { 
        UpdateUI(); 

        if (starterAssetsInputs == null && playerController != null)
        {
            starterAssetsInputs = playerController.GetComponent<StarterAssetsInputs>();
        }
    }

    private void Update()
    {
        // Đổi Mode bằng F2 (Chỉ khi không chơi)
        if (!isGameActive && Keyboard.current != null && Keyboard.current.f2Key.wasPressedThisFrame)
        {
            selectedMode = (selectedMode == 1) ? 2 : 1;
            isGameOver = false; // Reset trạng thái game over khi đổi mode
            UpdateUI();
        }

        // Logic Pause Game (ESC)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        if (isGameActive)
        {
            // Logic Timer
            if (isDotScene)
            {
                timer -= Time.deltaTime;
                if (timer <= 0) EndGame();
            }
            else
            {
                timer += Time.deltaTime;
            }

            if (gameUI != null) gameUI.UpdateTimer(timer);
        }
    }

    public void StartGame(int modeOverride = 0)
    {
        int modeToPlay = (modeOverride == 0) ? selectedMode : modeOverride;
        selectedMode = modeToPlay;

        isGameActive = true;
        isGameOver = false; // Tắt bảng kết quả đi
        
        botsKilled = 0; shotsFired = 0; shotsHit = 0; 
        botsSpawned = 0; currentActiveTargets = 0;

        // Setup Timer
        if (isDotScene)
        {
            if (selectedMode == 1) timer = dotMode1Duration;
            else timer = dotMode2Duration;
        }
        else
        {
            timer = 0f;
        }

        UpdateUI();

        // Spawn ban đầu
        if (isDotScene && selectedMode == 1)
        {
            for (int i = 0; i < dotMode1Concurrent; i++) SpawnTarget();
        }
        else
        {
            SpawnTarget();
        }
    }

    public void RestartGame(int mode) { CancelGame(); StartGame(mode); }

    public void CancelGame()
    {
        isGameActive = false;
        isGameOver = false; // Cancel thì reset về menu luôn
        StopAllCoroutines();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); 
        foreach (GameObject obj in enemies) Destroy(obj);

        timer = 0f; botsKilled = 0; shotsFired = 0; shotsHit = 0;
        UpdateUI();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Đóng băng thời gian
        pausePanel.SetActive(true);
        
        // Mở khóa chuột để bấm nút
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (starterAssetsInputs != null) 
        {
            starterAssetsInputs.cursorInputForLook = false;
            starterAssetsInputs.look = Vector2.zero;
        }

        // Nếu có slider, cập nhật giá trị hiện tại của chuột vào slider
        if(sensitivitySlider != null && playerController != null)
        {
            // Tạm thời bỏ lắng nghe sự kiện
            sensitivitySlider.onValueChanged.RemoveAllListeners(); 
            
            // Set giá trị
            sensitivitySlider.value = playerController.RotationSpeed;
            
            // Gán lại sự kiện (Dynamic)
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Chạy lại thời gian
        pausePanel.SetActive(false);

        // Khóa chuột lại để bắn tiếp
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (starterAssetsInputs != null) 
        {
            starterAssetsInputs.cursorInputForLook = true;
            // Reset input quay chuột để tránh bị giật camera cái vèo
            starterAssetsInputs.look = Vector2.zero; 
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Nhớ trả lại thời gian trước khi chuyển scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    // Hàm này sẽ gắn vào sự kiện "On Value Changed" của Slider
    public void SetSensitivity(float value)
    {
        if (playerController != null)
        {
            playerController.RotationSpeed = value;
        }
    }

    public void SpawnTarget()
    {
        if (!isGameActive) return;
        
        // [QUAN TRỌNG] Check điều kiện dừng TRƯỚC khi spawn
        // Bot Mode 1: Nếu đã spawn đủ 30 con rồi thì không spawn nữa -> End Game
        if (!isDotScene && selectedMode == 1)
        {
            if (botsSpawned >= botMode1TotalSpawns)
            {
                // Chỉ End Game khi con bot cuối cùng đã biến mất (để tránh End ngay khi con thứ 30 vừa hiện ra)
                if (currentActiveTargets <= 0) EndGame();
                return;
            }
        }
        // Bot Mode 2: Check theo số kill
        if (!isDotScene && selectedMode == 2 && botsKilled >= botMode2TargetKills) 
        {
            EndGame(); 
            return;
        }

        // --- 1. XỬ LÝ VỊ TRÍ (Fix lỗi Bot bay) ---
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        
        float y = 0;
        if (isDotScene)
        {
            // Dot: Random độ cao
            y = Random.Range(bounds.min.y, bounds.max.y);
        }
        else
        {
            // Bot: Luôn ở mặt đất (min Y của box collider)
            y = bounds.min.y;
        }

        Vector3 spawnPos = new Vector3(x, y, z);
        GameObject prefabToSpawn = isDotScene ? dotPrefab : botPrefab;
        Quaternion spawnRot = Quaternion.Euler(0, 180, 0);

        GameObject newTarget = Instantiate(prefabToSpawn, spawnPos, spawnRot);
        
        botsSpawned++;      // Tăng tổng số đã sinh ra
        currentActiveTargets++; // Tăng số lượng đang sống

        // --- 2. CẤU HÌNH LOGIC DI CHUYỂN ---
        if (isDotScene)
        {
            DotMovement dotMove = newTarget.GetComponent<DotMovement>();
            if (dotMove != null) dotMove.isMovingAllowed = (selectedMode == 2);
        }
        else
        {
            BotMovement botMove = newTarget.GetComponent<BotMovement>();
            if (botMove != null) botMove.isMovingAllowed = (selectedMode == 2);
            
            // Bot Mode 1: Tự hủy sau 1 khoảng thời gian (Reflex)
            if (selectedMode == 1) StartCoroutine(BotLifeCycleRoutine(newTarget));
        }

        UpdateUI();
    }

    // Coroutine đếm ngược cho Bot Mode 1
    IEnumerator BotLifeCycleRoutine(GameObject botRef)
    {
        yield return new WaitForSeconds(botMode1Duration);
        
        // Nếu hết giờ mà bot vẫn còn (chưa bị bắn chết)
        if (isGameActive && botRef != null)
        {
            Destroy(botRef);
            currentActiveTargets--; // Giảm số lượng đang sống
            
            // Spawn con tiếp theo (Miss cũng tính là qua lượt)
            SpawnTarget(); 
            
            // Cập nhật lại UI (để check xem đã hết 30 con chưa)
            // Nếu đây là con thứ 30 vừa biến mất -> SpawnTarget sẽ gọi EndGame
            if (!isDotScene && selectedMode == 1 && botsSpawned >= botMode1TotalSpawns)
            {
                EndGame();
            }
        }
    }

    public void RegisterShot() { if(isGameActive) { shotsFired++; UpdateUI(); } }
    
    public void RegisterHit()  { if(isGameActive) { shotsHit++; UpdateUI(); } }

    public void RegisterKill()
    {
        if (!isGameActive) return;
        
        botsKilled++;
        currentActiveTargets--; // Giảm số lượng đang sống

        // Logic Spawn tiếp theo
        if (isDotScene && selectedMode == 1)
        {
            SpawnTarget(); // Gridshot: Chết 1 đẻ 1
        }
        else
        {
            // Bot Mode: Chết con này ra con kia
            SpawnTarget();
        }
        UpdateUI();
    }

    private void EndGame()
    {
        isGameActive = false;
        isGameOver = true; // [FIX] Bật cờ Game Over để giữ bảng điểm
        
        StopAllCoroutines();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemies) Destroy(obj);
        
        UpdateUI(); // Cập nhật lần cuối để hiện bảng kết quả
        Debug.Log("=== GAME OVER ===");
    }

    void UpdateUI()
    {
        if (gameUI == null) return;

        // --- [FIX] LOGIC HIỂN THỊ UI ---
        
        // 1. Nếu đang Game Over (Vừa chơi xong) -> GIỮ NGUYÊN ĐIỂM SỐ
        if (isGameOver)
        {
            // gameUI.timerText.text = "FINISHED!";
            // Không return, để nó chạy xuống dưới cập nhật điểm lần cuối
        }
        // 2. Nếu đang ở Menu (Chưa chơi, và không phải vừa xong game)
        else if (!isGameActive)
        {
            gameUI.timerText.text = "PRESS START";
            if(isDotScene)
                gameUI.scoreText.text = (selectedMode == 1) ? "MODE: GRIDSHOT" : "MODE: TRACKING";
            else 
                gameUI.scoreText.text = (selectedMode == 1) ? "MODE: REFLEX" : "MODE: TIME ATTACK";
            
            gameUI.accuracyText.text = "F2 Change Mode";
            return; // Thoát luôn, không hiện điểm số 0/0 làm gì
        }

        // 3. Hiển thị thông số khi đang chơi (hoặc khi Game Over)
        float accuracy = (shotsFired > 0) ? (float)shotsHit / shotsFired * 100f : 0;
        
        // Chỉ update timer nếu đang chơi, nếu Game Over thì giữ chữ "FINISHED!"
        if (isGameActive) gameUI.UpdateTimer(timer); 
        
        gameUI.UpdateAccuracy(accuracy);

        if (isDotScene)
        {
            if (selectedMode == 2) gameUI.scoreText.text = $"Score: {shotsHit}";
            else gameUI.UpdateScore(botsKilled, 0);
        }
        else // Bot Scene
        {
            if (selectedMode == 1)
                // Mode Reflex: Hiện số Bot đã giết / Tổng số đã sinh ra (hoặc tổng giới hạn 30)
                gameUI.scoreText.text = $"Score: {botsKilled}/{botMode1TotalSpawns}";
            else
                gameUI.UpdateScore(botsKilled, botMode2TargetKills);
        }
    }
}