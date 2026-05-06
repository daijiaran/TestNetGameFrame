using System.Collections.Generic;
using Shared.DJRNetLib.Packet;

public interface IWorldStateProvider
{
    /// <summary>
    /// 获取当前所有玩家的状态数据。
    /// </summary>
    IEnumerable<UserPositionAndStatusPacket> GetPlayerStates();

    /// <summary>
    /// 获取当前所有场景物体的状态数据。
    /// </summary>
    IEnumerable<ScenesItemDataPacket> GetSceneStates();
}
