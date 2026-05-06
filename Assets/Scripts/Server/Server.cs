using UnityEngine;

public class Server : SingelBase<Server>
{
    private IPlayerManager playerManager;
    private ISceneObjectManager sceneObjectManager;
    private INetworkService networkService;
    private IClientRegistry clientRegistry;
    private IServerCommandHandler commandHandler;
    private IWorldStateProvider worldStateProvider;
    private ServerStateSyncService stateSyncService;

    public ServiceUpdate serviceUpdate;
    public ServerAllPlayerManager serverAllPlayerManager;
    public ServerAllitemManager serverAllitemManager;
    public MonsterSpawn monsterSpawn;

    /// <summary>
    /// 在对象唤醒时初始化服务器单例基础状态。
    /// </summary>
    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// 启动服务器核心服务并初始化各个管理模块。
    /// </summary>
    private void Start()
    {
        Debug.Log("服务器开始运行！！！");

        serviceUpdate = new ServiceUpdate();
        serverAllPlayerManager = gameObject.AddComponent<ServerAllPlayerManager>();
        serverAllitemManager = gameObject.AddComponent<ServerAllitemManager>();
        monsterSpawn = gameObject.AddComponent<MonsterSpawn>();

        playerManager = serverAllPlayerManager;
        sceneObjectManager = serverAllitemManager;
        networkService = serviceUpdate;

        clientRegistry = new ClientRegistry();
        commandHandler = new ServerCommandHandler(clientRegistry, playerManager, sceneObjectManager, networkService);
        worldStateProvider = new ServerWorldStateProvider(playerManager, sceneObjectManager);
        stateSyncService = new ServerStateSyncService(networkService, clientRegistry, worldStateProvider);

        serviceUpdate.Initialize(commandHandler, clientRegistry);
        monsterSpawn.Initialize(playerManager, sceneObjectManager, networkService);
        monsterSpawn.GameSpawnInit();
    }

    /// <summary>
    /// 供 Unity 的 Update 调用，持续处理网络消息并同步世界状态。
    /// </summary>
    private void Update()
    {
        if (serviceUpdate == null || stateSyncService == null)
        {
            return;
        }

        serviceUpdate.Update();
        stateSyncService.BroadcastSnapshot();
    }
}
