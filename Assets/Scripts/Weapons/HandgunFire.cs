using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] AudioSource gunFire;
    [SerializeField] GameObject handgun;
    [SerializeField] bool canFire = true;
    [SerializeField] GameObject extraCross;
    [SerializeField] AudioSource emptyGunSound;
    [SerializeField] GameObject muzzleFlash;
    public float toTarget;
    // Update is called once per frame
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
        toTarget = PlayerCasting.distanceFromTarget;
        gunFire.Play();
        extraCross.SetActive(true);
        GlobalAmmo.handgunAmmoCount -= 1;
        handgun.GetComponent<Animator>().Play("HandgunFire");
        muzzleFlash.SetActive(true);
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
