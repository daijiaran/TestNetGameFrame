using System;
using System.Collections.Generic;
using System.Net;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;

public interface IPlayerManager
{
    Dictionary<string, PlayerInstance> AllPlayerInstance { get; }
    Dictionary<string, UserPositionAndStatusPacket> AllPlayerInstancesUserPositionPackets { get; }
    
    /// <summary>
    /// 根据客户端加入信息创建一个新的玩家实例。
    /// </summary>
    void CreatePlayerInstance(string clientKey, UserJoinPacket userJoinPacket);

    /// <summary>
    /// 移除指定客户端对应的玩家实例。
    /// </summary>
    void RemovePlayer(string clientKey);

    /// <summary>
    /// 处理指定玩家的移动输入。
    /// </summary>
    void HandlePlayerMove(string clientKey, UserMovePacket movePacket);

    /// <summary>
    /// 处理指定玩家的攻击输入。
    /// </summary>
    void TriggerPlayerAtacck(string clientKey, EndPoint remoteClient, UserAttackPacket userAttackPacket);
}
