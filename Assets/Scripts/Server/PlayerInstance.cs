
using System;
using TMPro;
using UnityEngine;

public class PlayerInstance: MonoBehaviour
{
    public string PlayerName;
    public Rigidbody rigidbody;
    public Collider collider;
    public float moveSpeed = 5f; // 服务器端的移动速度

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
    }

    public void ApplyInput(float h, float v)
    {
        // 关键点：
        // 1. 接收纯输入 (H, V)，而不是位置。
        // 2. 在服务器上计算物理移动。
        // 3. 结果会自动通过 ServerAllPlayerManager 的 UpdatePlayerData 也就是 UserPositionPacket 同步回客户端。
    
        Vector3 moveDir = new Vector3(h, 0, v);
        if (moveDir.magnitude > 0.1f)
        {
            moveDir = moveDir.normalized;
        }
    
        // 直接修改速度
        if (rigidbody != null)
        {
            rigidbody.linearVelocity = new Vector3(moveDir.x * moveSpeed, rigidbody.linearVelocity.y, moveDir.z * moveSpeed);
        
            // 也可以处理旋转，让角色朝向移动方向
            if (moveDir != Vector3.zero)
            {
                transform.forward = moveDir; 
            }
        }
    }
    
    
    
}