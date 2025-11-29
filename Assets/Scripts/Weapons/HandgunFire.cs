using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HandgunFire : MonoBehaviour
{
    [SerializeField] AudioSource gunFire;
    [SerializeField] GameObject handgun;
    [SerializeField] bool canFire = true;
    [SerializeField] GameObject extraCross;
    [SerializeField] AudioSource emptyGunSound;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] float weaponDamage = 50f;
    public float toTarget;

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (canFire == true)
            {
                if (GlobalAmmo.handgunAmmoCount == 0)
                {
                    canFire = false;
                    StartCoroutine(EmptyGun());
                }
                else
                {
                    canFire = false;
                    StartCoroutine(FiringGun());
                }
            }
        }
    }

    IEnumerator FiringGun()
    {
        // 1. Xử lý hiệu ứng bắn trước
        toTarget = PlayerCasting.distanceFromTarget;
        gunFire.Play();
        extraCross.SetActive(true);
        GlobalAmmo.handgunAmmoCount -= 1;
        handgun.GetComponent<Animator>().Play("HandgunFire");
        muzzleFlash.SetActive(true);

        // Báo GameManager là đã bắn 1 viên (chỉ tính nếu game đang chạy)
        GameManager.Instance.RegisterShot();

        // 2. Kiểm tra trúng cái gì
        GameObject target = PlayerCasting.targetObject;
        if (target != null)
        {
            // --- [LOGIC MỚI: Ưu tiên kiểm tra nút bấm] ---
            MenuButton button = target.GetComponent<MenuButton>();
            if (button != null)
            {
                button.OnHit(); // Kích hoạt nút (Start hoặc Cancel)
            }
            else 
            {
                // Nếu không phải nút thì mới kiểm tra xem có phải Bot không
                BotHitbox hitbox = target.GetComponent<BotHitbox>();
                if (hitbox != null)
                {
                    hitbox.OnHit(weaponDamage);
                    GameManager.Instance.RegisterHit();
                }
            }
        }

        yield return new WaitForSeconds(0.04f);
        muzzleFlash.SetActive(false);
        yield return new WaitForSeconds(0.46f);
        handgun.GetComponent<Animator>().Play("New State");
        extraCross.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        canFire = true;
    }

    IEnumerator EmptyGun()
    {
        emptyGunSound.Play();
        yield return new WaitForSeconds(0.6f);
        canFire = true;
    }
}