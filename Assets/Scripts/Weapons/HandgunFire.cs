using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class HandgunFire : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] float weaponDamage = 50f;
    [SerializeField] float fireRate = 0.25f;
    
    [Header("Ammo Settings")]
    public int maxAmmo = 12;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    public bool isReloading = false;
    public bool infiniteAmmo = false;

    [Header("References")]
    [SerializeField] AudioClip fireSound;   // [SỬA]
    [SerializeField] AudioClip reloadSound; // [SỬA]
    
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Animator gunAnim;
    [SerializeField] GameObject extraCross;
    [SerializeField] TextMeshProUGUI ammoText;

    private bool canFire = true;
    private AudioSource audioSource; // [MỚI]

    void Start()
    {
        currentAmmo = maxAmmo;
        // Tự tìm hoặc tạo AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        UpdateAmmoUI();
        if (GameManager.Instance != null && GameManager.Instance.isDotScene) infiniteAmmo = true;
    }

    void OnEnable()
    {
        isReloading = false;
        canFire = true;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused) return;

        if (isReloading) return;

        bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if ((currentAmmo <= 0 && !infiniteAmmo) || (Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < maxAmmo && !infiniteAmmo))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && canFire && currentAmmo > 0 && !isPointerOverUI)
        {
            StartCoroutine(FiringGun());
        }
    }

    IEnumerator FiringGun()
    {
        canFire = false;
        if (!infiniteAmmo) { currentAmmo--; UpdateAmmoUI(); }

        // [SỬA]
        if(audioSource && fireSound) audioSource.PlayOneShot(fireSound);
        
        extraCross.SetActive(true);
        muzzleFlash.SetActive(true);
        
        gunAnim.Play("Fire", -1, 0f); // Check tên "Fire"

        if (GameManager.Instance != null) GameManager.Instance.RegisterShot();

        GameObject target = PlayerCasting.targetObject;
        if (target != null)
        {
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

        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
        extraCross.SetActive(false);
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        
        // [SỬA]
        if(audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);
        
        gunAnim.Play("Reload", -1, 0f); // Check tên "Reload"

        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null) ammoText.text = infiniteAmmo ? "∞" : $"{currentAmmo} / {maxAmmo}";
    }
}