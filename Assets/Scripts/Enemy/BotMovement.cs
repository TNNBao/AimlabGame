using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float moveRange = 1.5f; // Phạm vi di chuyển (trái phải 1.5m)
    [SerializeField] float changeDirectionTime = 2.0f; // Bao lâu thì đổi hướng 1 lần

    [Header("State")]
    public bool isMovingAllowed = true; // Dùng để bật tắt bằng F3 sau này

    private Animator anim;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveParameter; // Giá trị gửi vào Animator (-1, 0, 1)

    void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position; // Ghi nhớ vị trí ban đầu làm gốc
        targetPosition = startPosition;
        
        // Bắt đầu quy trình suy nghĩ của Bot
        StartCoroutine(BotLogicRoutine());
    }

    void Update()
    {
        if (!isMovingAllowed)
        {
            StopMoving();
            return;
        }

        MoveBot();
        UpdateAnimator();
    }

    void MoveBot()
    {
        // Di chuyển Bot từ từ đến vị trí mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        // Tính toán xem đang đi về bên trái hay phải so với đích đến
        float direction = targetPosition.x - transform.position.x;

        // Nếu khoảng cách tới đích rất nhỏ (< 0.1) thì coi như đứng yên (0)
        // Nếu > 0.1 thì là đi phải (1), < -0.1 là đi trái (-1)
        float targetValue = 0;
        
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            targetValue = direction > 0 ? 1f : -1f;
        }

        // Lerp giá trị để animation chuyển mượt mà (không bị giật cục)
        moveParameter = Mathf.Lerp(moveParameter, targetValue, Time.deltaTime * 5f);
        anim.SetFloat("MoveX", moveParameter);
    }

    void StopMoving()
    {
        targetPosition = transform.position;
        anim.SetFloat("MoveX", 0);
    }

    IEnumerator BotLogicRoutine()
    {
        while (true)
        {
            if (isMovingAllowed)
            {
                // 50% cơ hội đứng yên, 50% cơ hội di chuyển
                float randomChance = Random.Range(0f, 1f);

                if (randomChance > 0.4f) 
                {
                    // Chọn một điểm ngẫu nhiên trên trục X trong phạm vi cho phép
                    float randomX = Random.Range(0, 2) == 0 ? -moveRange : moveRange;
                    targetPosition = startPosition + new Vector3(randomX, 0, 0);
                }
                else
                {
                    // Đứng yên tại chỗ một chút
                    targetPosition = transform.position;
                }
            }

            // Chờ một khoảng thời gian ngẫu nhiên rồi mới quyết định tiếp
            float waitTime = Random.Range(0.5f, changeDirectionTime);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
