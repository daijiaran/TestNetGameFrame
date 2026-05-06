using Shared.DJRNetLib.Packet;

public class ServerStateSyncService
{
    private readonly INetworkService networkService;
    private readonly IClientRegistry clientRegistry;
    private readonly IWorldStateProvider worldStateProvider;

    /// <summary>
    /// 创建世界状态同步服务并保存所需依赖。
    /// </summary>
    public ServerStateSyncService(
        INetworkService networkService,
        IClientRegistry clientRegistry,
        IWorldStateProvider worldStateProvider)
    {
        this.networkService = networkService;
        this.clientRegistry = clientRegistry;
        this.worldStateProvider = worldStateProvider;
    }

    /// <summary>
    /// 向所有客户端广播当前玩家与场景物体快照。
    /// </summary>
    public void BroadcastSnapshot()
    {
        foreach (var clientKvp in clientRegistry.GetAll())
        {
            var receiverEndPoint = clientKvp.Value;

            foreach (UserPositionAndStatusPacket playerState in worldStateProvider.GetPlayerStates())
            {
                networkService.SendToClient(playerState, receiverEndPoint);
            }

            foreach (ScenesItemDataPacket sceneState in worldStateProvider.GetSceneStates())
            {
                networkService.SendToClient(sceneState, receiverEndPoint);
            }
        }
    }
}
