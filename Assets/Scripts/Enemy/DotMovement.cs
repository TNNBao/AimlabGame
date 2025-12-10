using UnityEngine;

public class DotMovement : MonoBehaviour
{
    public bool isMovingAllowed = false;
    public float speed = 3f;
    
    private Vector3 targetPos;
    private Bounds movementBounds; // [MỚI] Lưu giới hạn vùng bay
    private bool hasBounds = false;

    void Update()
    {
        if (!isMovingAllowed || !hasBounds) return;

        // Di chuyển tới điểm mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Nếu đã đến nơi (hoặc rất gần) -> Chọn điểm mới
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            SetNewTarget();
        }
    }

    // [MỚI] Hàm này sẽ được GameManager gọi ngay khi Spawn
    public void SetMovementBounds(Bounds bounds)
    {
        movementBounds = bounds;
        hasBounds = true;
        SetNewTarget(); // Chọn điểm đến đầu tiên
    }

    void SetNewTarget()
    {
        if (!hasBounds) return;

        // Chọn tọa độ ngẫu nhiên TUYỆT ĐỐI nằm trong hộp
        float rX = Random.Range(movementBounds.min.x, movementBounds.max.x);
        float rY = Random.Range(movementBounds.min.y, movementBounds.max.y);
        float rZ = Random.Range(movementBounds.min.z, movementBounds.max.z);

        targetPos = new Vector3(rX, rY, rZ);
    }
}