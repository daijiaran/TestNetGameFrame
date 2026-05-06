using System.Collections.Generic;
using Shared.DJRNetLib.Packet;

public class ServerWorldStateProvider : IWorldStateProvider
{
    private readonly IPlayerManager playerManager;
    private readonly ISceneObjectManager sceneObjectManager;

    /// <summary>
    /// 创建服务器世界状态提供器。
    /// </summary>
    public ServerWorldStateProvider(IPlayerManager playerManager, ISceneObjectManager sceneObjectManager)
    {
        this.playerManager = playerManager;
        this.sceneObjectManager = sceneObjectManager;
    }

    /// <summary>
    /// 获取当前所有玩家的同步状态集合。
    /// </summary>
    public IEnumerable<UserPositionAndStatusPacket> GetPlayerStates()
    {
        return playerManager.AllPlayerInstancesUserPositionPackets.Values;
    }

    /// <summary>
    /// 获取当前所有场景物体的同步状态集合。
    /// </summary>
    public IEnumerable<ScenesItemDataPacket> GetSceneStates()
    {
        return sceneObjectManager.AllItemsTransData.Values;
    }
}
