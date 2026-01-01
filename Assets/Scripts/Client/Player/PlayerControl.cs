using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
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
        float h = 0;
        float v = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) h -= 1;
            if (Keyboard.current.dKey.isPressed) h += 1;
            if (Keyboard.current.sKey.isPressed) v -= 1;
            if (Keyboard.current.wKey.isPressed) v += 1;
        }

        // 只有当有输入时才发送，或者每一帧都发送（取决于设计，建议有输入才发以节省带宽）
        // 这里为了演示，只要有输入就发
        if (h != 0 || v != 0)
        {
            // 构造包
            UserMovePacket movePacket = new UserMovePacket(h, v);
            // 发送包 (需要通过 NetworkManager 调用 NetConect)
            // 假设你已经在 NetworkManager 里面写了一个 SendMove 的封装，或者直接访问
            NetworkManager.Instance.SendMoveToSever(movePacket); 
        }
    
        // 客户端本地也可以保留移动逻辑用于平滑表现（预测），但最终位置以服务器同步回来的 UserPositionPacket 为准
    }
}