using UnityEngine;

public class GlobalAmmo : MonoBehaviour
{
    public static int handgunAmmoCount = 10;
    [SerializeField] GameObject ammoDisplay;
    // Update is called once per frame
    void Update()
    {
        ammoDisplay.GetComponent<TMPro.TMP_Text>().text = "" + handgunAmmoCount;
    }
}
