using System;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class BulltControl : ScenesItemBase
{
    public Rigidbody rb;
    public Collider col;
    public float damageValue = 30;
    public float BulltVelocity = 10;
    
    private void Start()
    {
      
    }


    public override void InitOBJ()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        
        if (IsServer)
        {
            Destroy(gameObject, 5f); 
            col.isTrigger = true;
            Server.Instance.serverAllitemManager.AddAllItem(transform,PrefabsName);
        }
        else
        {
            rb.isKinematic = true;
            col.enabled = false;
        }
    }


    /// <summary>
    /// 当对象销毁的时候移除字典中的该对象
    /// </summary>
    private void OnDisable()
    {
        if (IsServer)
        {
            ScenesItemDataPacket scenesItemPacket = Server.Instance.serverAllitemManager.AllItemsTransData[CurrentInstanceID];
            scenesItemPacket.isDestroy = true;
            Server.Instance.serverAllitemManager.RemoveScenesItem(CurrentInstanceID);
            Server.Instance.serviceUpdate.SendToAllPlayerDestoryOBJ(PacketType.ScenesItem,scenesItemPacket);
        }
    }
     
    private void FixedUpdate()
    {
        if (!IsServer) return;
    
        float moveDistance = BulltVelocity * Time.fixedDeltaTime;
        Vector3 direction = transform.forward;
    
        float bulletRadius = 0.3f; // 子弹的粗细
        if (Physics.SphereCast(transform.position, bulletRadius, direction, out RaycastHit hit, moveDistance))
        {
            // 确保击中的是玩家层或具有 PlayerInstance 脚本
            PlayerInstance player = hit.collider.GetComponent<PlayerInstance>();
            if (player != null)
            {
                // 将子弹移至撞击点（可选，视觉更真实）
                transform.position = hit.point;
            
                player.PlayerLifeControl.TakeDamage(damageValue);
                Destroy(gameObject);
                Debug.Log("射线检测命中玩家！");
            }
        }
        else
        {
            // 如果没撞到，正常移动
            transform.position += direction * moveDistance;
        }
    }
}