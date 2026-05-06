using System.Net;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;

public interface IServerCommandHandler
{
    /// <summary>
    /// 处理客户端加入游戏请求。
    /// </summary>
    void HandleJoin(string clientKey, EndPoint remoteClient, UserJoinPacket packet);

    /// <summary>
    /// 处理客户端发送的移动请求。
    /// </summary>
    void HandleMove(string clientKey, UserMovePacket packet);

    /// <summary>
    /// 处理客户端发送的攻击请求。
    /// </summary>
    void HandleAttack(string clientKey, EndPoint remoteClient, UserAttackPacket packet);
}
