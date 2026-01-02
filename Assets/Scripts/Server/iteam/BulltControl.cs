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
        
        Debug.Log("Aaaaaaaaa");
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
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            PlayerInstance player = other.GetComponent<PlayerInstance>();
            if (player != null)
            {
                player.PlayerLifeControl.TakeDamage(damageValue);    
                Debug.Log("执行伤害玩家方法！！");
            }
        }
    }
}