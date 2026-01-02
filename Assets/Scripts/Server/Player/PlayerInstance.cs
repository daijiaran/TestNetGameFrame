
using System;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using TMPro;
using UnityEngine;

public class PlayerInstance: MonoBehaviour
{
    public string PlayerName;
    public string PlayerIp;
    public Rigidbody rigidbody;
    public Collider collider;
    public float moveSpeed = 5f; // 服务器端的移动速度
    [Header("枪口的面向方向")]
    public Transform FaceDirection;
    public Transform BullettPos;
    public PlayerLifeControl PlayerLifeControl =  new PlayerLifeControl();

    
    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
        PlayerLifeControl.IsDead += Die;
    }

    
    public void ApplyMoveInput(UserMovePacket movePacket)
    {
        Vector3 forward = new Vector3(movePacket.D_x, 0, movePacket.D_z);
        if (forward.sqrMagnitude < 0.0001f)
            return;

        forward.Normalize();
        
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        // 合成移动方向
        Vector3 moveDir = forward * movePacket.V + right * movePacket.H;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        if (rigidbody != null)
        {
            rigidbody.linearVelocity = new Vector3(
                moveDir.x * moveSpeed,
                rigidbody.linearVelocity.y,
                moveDir.z * moveSpeed
            );

            if (moveDir.sqrMagnitude > 0.001f)
            {
                transform.forward = moveDir;
            }
        }
        
        
        FaceDirection.forward = new Vector3(movePacket.Attack_x, 0, movePacket.Attack_z);
        
    }


    public void ApplyAttackInput(UserAttackPacket attackPacket)
    {
        if (attackPacket.BulltType == 1)
        {
            // 1. 加载资源 (建议：不要在Update/攻击时实时Load，最好在Start中预加载缓存，否则会卡顿)
            GameObject prefab = Resources.Load<GameObject>("Prefabs/"+attackPacket.Prefabsname);
            if (prefab == null) 
            {
                Debug.LogError("找不到子弹预制体！请检查路径：Prefabs/"+attackPacket.Prefabsname);
                return;
            }
            
            GameObject bulletObj = Instantiate(prefab, BullettPos.position, FaceDirection.rotation);
            
            BulltControl bulletScript = bulletObj.GetComponent<BulltControl>();
            bulletScript.IsServer = true;
            bulletScript.InitOBJ();
            
            bulletScript.PrefabsName = attackPacket.Prefabsname;
            Rigidbody bulletRb = bulletObj.GetComponent<Rigidbody>();

            if (bulletRb != null && bulletScript != null)
            {
                // 确保速度不为0，如果脚本里没填，给个默认值 20
                float speed = bulletScript.BulltVelocity > 0 ? bulletScript.BulltVelocity : 20f;
            
                // Unity 6 写法
                bulletRb.linearVelocity = FaceDirection.forward * speed; 
            
            }

            //忽略子弹与发射者(玩家)之间的碰撞
            //假设当前脚本挂在玩家身上，且玩家有 Collider
            Collider playerCollider = GetComponent<Collider>();
            Collider bulletCollider = bulletObj.GetComponent<Collider>();
        
            if (playerCollider != null && bulletCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, bulletCollider);
            }
        }
    }


    public void Die()
    {
        Destroy(this.gameObject);
    }
    
    
    
    

    private void OnDisable()
    {
        UserPositionAndStatusPacket userStatusPacket =  Server.Instance.serverAllPlayerManager.AllPlayerInstancesUserPositionPackets[PlayerIp];
        Server.Instance.serverAllPlayerManager.AllPlayerInstance.Remove(PlayerIp);
        Server.Instance.serverAllPlayerManager.AllPlayerInstancesUserPositionPackets.Remove(PlayerIp);
        userStatusPacket.isDead = true;
        //发送最后一条销毁自己的信息
        Server.Instance.serviceUpdate.SendToAllPlayerDestoryOBJ(PacketType.PositionAndStatus,userStatusPacket);
    }
    
    
}