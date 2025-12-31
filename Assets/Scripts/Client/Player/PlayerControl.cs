using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; // 【必须】引用新版命名空间

public class PlayerControl : MonoBehaviour
{
    public Collider collider;
    public Rigidbody rigidbody;
    public TextMeshProUGUI PlayerName;
    public float moveSpeed = 5f;
    public bool isCurrentPlayer = false;

    private void Start()
    {
        collider = GetComponent<Collider>(); 
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isCurrentPlayer)
        {
            Move();
        }
    }
    
    private void Move()
    {
        // ==========================================
        // 【修改】使用新版 Input System 直接读取键盘
        // ==========================================
        
        float h = 0;
        float v = 0;

        // 检查键盘是否存在 (防止没有键盘时报错)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) h -= 1;
            if (Keyboard.current.dKey.isPressed) h += 1;
            if (Keyboard.current.sKey.isPressed) v -= 1;
            if (Keyboard.current.wKey.isPressed) v += 1;
        }

        // 下面的移动逻辑保持不变
        Vector3 moveDir = new Vector3(h, 0, v);

        if (moveDir.magnitude > 0.1f)
        {
            moveDir = moveDir.normalized;
        }

        rigidbody.linearVelocity = new Vector3(moveDir.x * moveSpeed, rigidbody.linearVelocity.y, moveDir.z * moveSpeed);
    }
}