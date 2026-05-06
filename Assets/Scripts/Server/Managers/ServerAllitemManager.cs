using System;
using System.Collections.Generic;
using MyNetGame.ServerScenesOBJ;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class ServerAllitemManager : MonoBehaviour, ISceneObjectManager
{
    private int lastItemIndex = 0;
    public Dictionary<int, ScenesItemDataPacket> AllItemsTransData { get; } = new Dictionary<int, ScenesItemDataPacket>();
    public Dictionary<int, Transform> AllItemInstance = new Dictionary<int, Transform>();

    /// <summary>
    /// 供 Unity 的 Update 调用，持续刷新场景物体的同步数据。
    /// </summary>
    private void Update()
    {
        AllItemTransDataUpdate();
    }

    /// <summary>
    /// 将新的场景物体注册到服务器同步管理中。
    /// </summary>
    public void AddAllItem(Transform IteamTransform, string ItemName)
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
        ServerScenesObjectBase scenesObject = IteamTransform.GetComponent<ServerScenesObjectBase>();
        scenesObject.CurrentInstanceID = lastItemIndex;
        
        lastItemIndex++;
    }

    /// <summary>
    /// 移除指定编号的场景物体及其同步数据。
    /// </summary>
    public void RemoveScenesItem(int IteamID)
    {
        AllItemInstance.Remove(IteamID);
        AllItemsTransData.Remove(IteamID);
    }
    
    /// <summary>
    /// 更新所有已注册场景物体的位置与旋转数据。
    /// </summary>
    private void AllItemTransDataUpdate()
    {
        foreach (var key in AllItemInstance)
        {
            Transform IteamTransform = AllItemInstance[key.Key];
            String Name = AllItemsTransData[key.Key].ItemName;
            
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
