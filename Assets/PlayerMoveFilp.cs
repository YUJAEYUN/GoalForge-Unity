using UnityEngine;

public class PlayerMoveFlip : MonoBehaviour
{
    public float moveSpeed = 5f;

    [SerializeField] private Transform visualRoot;    // "Visual" 자식
    private Animator anim;

    void Awake()
    {
        // Visual 자동 찾기 (Inspector에서 안 넣었을 때)
        if (visualRoot == null)
            visualRoot = transform.Find("Visual");

        if (visualRoot == null)
        {
            Debug.LogError("Visual 오브젝트를 찾지 못했습니다! PlayerMoveFlip에서 visualRoot를 확인하세요.");
            return;
        }

        anim = visualRoot.GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Visual에 Animator가 없습니다.");
        }
    }

    void Update()
    {
        // -1(왼쪽), 0, 1(오른쪽)
        float moveX = Input.GetAxisRaw("Horizontal");
        bool isMoving = Mathf.Abs(moveX) > 0.01f;

        // 1. Player(루트) 이동
        Vector3 dir = new Vector3(moveX, 0f, 0f);
        transform.position += dir * moveSpeed * Time.deltaTime;

        // 2. Visual 전체 좌우 반전
        if (visualRoot != null)
        {
            if (moveX > 0f)
            {
                visualRoot.localScale = new Vector3(1f, 1f, 1f);   // 오른쪽
            }
            else if (moveX < 0f)
            {
                visualRoot.localScale = new Vector3(-1f, 1f, 1f);  // 왼쪽
            }
        }

        // 3. 애니메이션 상태 전환용 Speed (0 또는 1만 사용)
        if (anim != null)
        {
            anim.SetFloat("Speed", isMoving ? 1f : 0f);
        }
    }
}
