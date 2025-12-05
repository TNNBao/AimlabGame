using UnityEngine;

public class DotMovement : MonoBehaviour
{
    public bool isMovingAllowed = false;
    public float speed = 3f;
    public float moveRange = 2f;
    
    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.position;
        SetNewTarget();
    }

    void Update()
    {
        if (!isMovingAllowed) return;

        // Di chuyển tới điểm mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Nếu đã đến nơi -> Chọn điểm mới
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            SetNewTarget();
        }
    }

    void SetNewTarget()
    {
        // Random điểm mới quanh vị trí ban đầu
        float rX = Random.Range(-moveRange, moveRange);
        float rY = Random.Range(-moveRange, moveRange); // Dot bay lên xuống được
        targetPos = startPos + new Vector3(rX, rY, 0);
    }
}