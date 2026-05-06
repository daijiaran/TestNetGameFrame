using System.Collections.Generic;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public interface ISceneObjectManager
{
    Dictionary<int, ScenesItemDataPacket> AllItemsTransData { get; }
    
    /// <summary>
    /// 将场景物体添加到服务器同步管理中。
    /// </summary>
    void AddAllItem(Transform itemTransform, string itemName);

    /// <summary>
    /// 从服务器同步管理中移除指定场景物体。
    /// </summary>
    void RemoveScenesItem(int itemID);
}
