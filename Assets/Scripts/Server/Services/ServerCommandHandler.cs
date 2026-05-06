using System.Net;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class ServerCommandHandler : IServerCommandHandler
{
    private readonly IClientRegistry clientRegistry;
    private readonly IPlayerManager playerManager;
    private readonly ISceneObjectManager sceneObjectManager;
    private readonly INetworkService networkService;

    /// <summary>
    /// 创建服务端命令处理器并注入所需依赖。
    /// </summary>
    public ServerCommandHandler(
        IClientRegistry clientRegistry,
        IPlayerManager playerManager,
        ISceneObjectManager sceneObjectManager,
        INetworkService networkService)
    {
        this.clientRegistry = clientRegistry;
        this.playerManager = playerManager;
        this.sceneObjectManager = sceneObjectManager;
        this.networkService = networkService;
    }

    /// <summary>
    /// 处理客户端加入游戏请求并广播新玩家信息。
    /// </summary>
    public void HandleJoin(string clientKey, EndPoint remoteClient, UserJoinPacket packet)
    {
        clientRegistry.Upsert(clientKey, remoteClient);

        if (!playerManager.AllPlayerInstance.ContainsKey(clientKey))
        {
            playerManager.CreatePlayerInstance(clientKey, packet);

            if (playerManager.AllPlayerInstance.TryGetValue(clientKey, out var playerInstance))
            {
                playerInstance.Initialize(playerManager, sceneObjectManager, networkService);
            }
        }

        packet.Ip = clientKey;
        if (string.IsNullOrEmpty(packet.Ip))
        {
            Debug.LogError("服务端广播加入消息失败，玩家 IP 为空。");
            return;
        }

        networkService.SendToAllPlayerDestoryOBJ(PacketType.Join, packet);
    }

    /// <summary>
    /// 处理客户端发送的移动指令。
    /// </summary>
    public void HandleMove(string clientKey, UserMovePacket packet)
    {
        playerManager.HandlePlayerMove(clientKey, packet);
    }

    /// <summary>
    /// 处理客户端发送的攻击指令。
    /// </summary>
    public void HandleAttack(string clientKey, EndPoint remoteClient, UserAttackPacket packet)
    {
        playerManager.TriggerPlayerAtacck(clientKey, remoteClient, packet);
    }
}
