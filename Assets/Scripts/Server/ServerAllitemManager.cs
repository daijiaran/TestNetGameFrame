
using System;
using System.Collections.Generic;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class ServerAllitemManager : MonoBehaviour
{
    private int lastItemIndex = 0;
    public Dictionary<int,ScenesItemDataPacket> AllItemsTransData = new Dictionary<int, ScenesItemDataPacket>();
    public Dictionary<int,Transform> AllItemInstance = new Dictionary<int, Transform>();

    private void Update()
    {
        AllItemTransDataUpdate();
    }

    /// <summary>
    /// 添加场景中物品
    /// </summary>
    /// <param name="IteamTransform"></param>
    /// <param name="ItemName"></param>
    public void AddAllItem(Transform IteamTransform,string ItemName)
    {
        AllItemInstance.Add(lastItemIndex, IteamTransform);
        
        ScenesItemDataPacket IteamTansData = new ScenesItemDataPacket
            (
                ItemName,
                lastItemIndex,
                IteamTransform.position.x,
                IteamTransform.position.y,
                IteamTransform.position.z,
                IteamTransform.rotation.eulerAngles.x,
                IteamTransform.rotation.eulerAngles.y,
                IteamTransform.rotation.eulerAngles.z
            );
        
        AllItemsTransData.Add(lastItemIndex, IteamTansData);
        ScenesItemBase scenes_Item = IteamTransform.GetComponent<ScenesItemBase>();
        scenes_Item.CurrentInstanceID = lastItemIndex;
        
        lastItemIndex++;
    }

    /// <summary>
    /// 移除指定对象
    /// </summary>
    /// <param name="IteamID"></param>
    public void RemoveScenesItem(int IteamID)
    {
        AllItemInstance.Remove(IteamID);
        AllItemsTransData.Remove(IteamID);
    }
    
    


    /// <summary>
    /// 持续更新每个物品的位置数据
    /// </summary>
    private void AllItemTransDataUpdate()
    {
        foreach (var key in AllItemInstance)
        {
            Transform IteamTransform = AllItemInstance[key.Key];
            String Name = AllItemsTransData[key.Key].ItemName;
            
            //构造新的物体数据包
            ScenesItemDataPacket NewIteamTansData = new ScenesItemDataPacket
            (
                Name,
                key.Key,
                IteamTransform.position.x,
                IteamTransform.position.y,
                IteamTransform.position.z,
                IteamTransform.rotation.eulerAngles.x,
                IteamTransform.rotation.eulerAngles.y,
                IteamTransform.rotation.eulerAngles.z
            );
            
            AllItemsTransData[key.Key] = NewIteamTansData;
            
        }
    }
    
   
}