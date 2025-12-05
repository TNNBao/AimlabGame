using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject pistolObject;
    public GameObject rifleObject;

    void Start()
    {
        // Mặc định cầm Pistol
        EquipPistol();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            EquipPistol();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            EquipRifle();
        }
    }

    void EquipPistol()
    {
        pistolObject.SetActive(true);
        rifleObject.SetActive(false);
        Debug.Log("Đã chuyển sang Pistol");
    }

    void EquipRifle()
    {
        pistolObject.SetActive(false);
        rifleObject.SetActive(true);
        Debug.Log("Đã chuyển sang Rifle");
    }
}