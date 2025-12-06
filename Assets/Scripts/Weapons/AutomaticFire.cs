using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class AutomaticFire : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] float fireRate = 0.1f; 
    [SerializeField] float weaponDamage = 40f;
    [Tooltip("Độ lệch tâm: 0 là bắn chuẩn 100% như Laser. Nên để 0.01 - 0.02")]
    [SerializeField] float accuracySpread = 0.01f; // [GIẢM XUỐNG CÒN 0.01]

    [Header("Ammo Settings")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2.0f;
    public bool isReloading = false;
    public bool infiniteAmmo = false;

    [Header("References")]
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Animator gunAnim;
    [SerializeField] GameObject extraCross;
    [SerializeField] TextMeshProUGUI ammoText;

    private bool isFiring = false;
    private float nextFireTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        UpdateAmmoUI();
        if (GameManager.Instance != null && GameManager.Instance.isDotScene) infiniteAmmo = true;
    }

    void OnEnable()
    {
        isReloading = false;
        isFiring = false;
        nextFireTime = 0f; // Reset timer để rút súng ra là bắn được ngay
        UpdateAmmoUI();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused) return;

        if (isReloading) return;

        // Xử lý nạp đạn
        if ((currentAmmo <= 0 && !infiniteAmmo) || (Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < maxAmmo && !infiniteAmmo))
        {
            StartCoroutine(Reload());
            return;
        }

        bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        // Xử lý Input chuột
        if (Mouse.current.leftButton.isPressed && currentAmmo > 0 && !isPointerOverUI)
        {
            isFiring = true;
        }
        else
        {
            isFiring = false;
            // [MẸO] Reset nhẹ timer khi nhả chuột để lần nhấp tiếp theo nhạy hơn (fix lỗi tap tap)
            if (Time.time < nextFireTime) nextFireTime = Time.time; 
        }

        // Logic bắn
        if (isFiring && Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireBullet()
    {
        if (!infiniteAmmo)
        {
            currentAmmo--;
            UpdateAmmoUI();
        }

        if(audioSource && fireSound) audioSource.PlayOneShot(fireSound);
        if(muzzleFlash) StartCoroutine(FlashEffect());
        if(gunAnim) gunAnim.Play("Fire", -1, 0f);
        if(extraCross) extraCross.SetActive(true);

        if (GameManager.Instance != null) GameManager.Instance.RegisterShot();

        // --- [LOGIC TÍNH TOÁN RAYCAST MỚI CHUẨN HƠN] ---
        float xSpread = Random.Range(-accuracySpread, accuracySpread);
        float ySpread = Random.Range(-accuracySpread, accuracySpread);

        // Tính hướng bắn: Hướng Camera + lệch phải/trái + lệch lên/xuống
        Vector3 shootDirection = Camera.main.transform.forward + (Camera.main.transform.right * xSpread) + (Camera.main.transform.up * ySpread);

        // [DEBUG] Vẽ tia laser màu đỏ trong Scene View để kiểm tra (chỉ hiện trong Scene, ko hiện trong Game)
        Debug.DrawRay(Camera.main.transform.position, shootDirection * 100f, Color.red, 2.0f);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, shootDirection, out hit))
        {
            GameObject target = hit.collider.gameObject;
            
            // Log tên vật thể bắn trúng để kiểm tra
            // Debug.Log("AR Hit: " + target.name); 

            if (target.GetComponent<MenuButton>() != null) target.GetComponent<MenuButton>().OnHit();
            else if (target.GetComponent<BotHitbox>() != null) {
                target.GetComponent<BotHitbox>().OnHit(weaponDamage);
                if (GameManager.Instance != null) GameManager.Instance.RegisterHit();
            }
            else if (target.GetComponent<DotTarget>() != null) {
                target.GetComponent<DotTarget>().OnHit();
                if (GameManager.Instance != null) GameManager.Instance.RegisterHit();
            }
        }
        
        Invoke("DisableCrosshair", fireRate * 0.8f);
    }

    // ... (Giữ nguyên các hàm Reload, FlashEffect, UpdateAmmoUI như cũ) ...
    IEnumerator Reload()
    {
        isReloading = true;
        if(audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);
        if (gunAnim) gunAnim.Play("Reload", -1, 0f); 
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    IEnumerator FlashEffect()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(fireRate * 0.5f);
        muzzleFlash.SetActive(false);
    }

    void DisableCrosshair() { if(extraCross) extraCross.SetActive(false); }
    
    void UpdateAmmoUI()
    {
        if (ammoText != null) ammoText.text = infiniteAmmo ? "∞ / ∞" : $"{currentAmmo} / {maxAmmo}";
    }
}