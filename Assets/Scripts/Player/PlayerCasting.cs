using UnityEngine;

public class PlayerCasting : MonoBehaviour
{
    public static float distanceFromTarget;
    public static GameObject targetObject;
    [SerializeField] float toTarget;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            distanceFromTarget = hit.distance;
            toTarget = hit.distance;
            targetObject = hit.collider.gameObject;
        }
        else
        {
            distanceFromTarget = 100000f;
            targetObject = null;
        }
        
    }
}
