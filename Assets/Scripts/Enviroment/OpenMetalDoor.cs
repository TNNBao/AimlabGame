using UnityEngine;
using UnityEngine.InputSystem;

public class OpenMetalDoor : MonoBehaviour
{
    public float maxInteractionDistance = 4f; 
    
    public GameObject actionDisplay;
    public GameObject actionText;
    public GameObject leftDoor;
    public GameObject rightDoor;

    private bool isDoorOpened = false; 

    void Update()
    {
        if (isDoorOpened)
        {
            return;
        }

        float currentDistance = PlayerCasting.distanceFromTarget; 
        GameObject target = PlayerCasting.targetObject;

        if (target == this.gameObject && currentDistance <= maxInteractionDistance)
        {
            actionDisplay.SetActive(true);
            actionText.SetActive(true);

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                isDoorOpened = true; 
                
                this.GetComponent<BoxCollider>().enabled = false;
                
                actionDisplay.SetActive(false);
                actionText.SetActive(false);
                
                leftDoor.GetComponent<Animator>().Play("LeftSlide");
                rightDoor.GetComponent<Animator>().Play("RightSlide");
            }
        }
        else
        {
            actionDisplay.SetActive(false);
            actionText.SetActive(false);
        }
    }
}