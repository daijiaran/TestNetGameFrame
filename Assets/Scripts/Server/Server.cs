using Unity.VisualScripting;
using UnityEngine;

public class Server : SingelBase<Server>
{
    public ServiceUpdate serviceUpdate;
    public ServerAllPlayerManager serverAllPlayerManager;
    
    void Start()
    {
        Debug.Log("服务器开始运行！！！");        
        
        // 【修正 1】必须先初始化网络服务
        // 这样当后面 Instantiate 触发 Awake 时，serviceUpdate 已经存在了
        serviceUpdate = new ServiceUpdate();
        transform.AddComponent<ServerAllPlayerManager>();
        serverAllPlayerManager = transform.GetComponent<ServerAllPlayerManager>();
    }

    void Update()
    {
        //同步当前服务器上的数据
        // 注意：确保 serverAllPlayerManager 和 serviceUpdate 都不为空
        if (serverAllPlayerManager != null && serviceUpdate != null)
        {
            serviceUpdate.playersData.Players = serverAllPlayerManager.AllPlayerInstancesUserPositionPackets;
            serviceUpdate.Update();
        }
    }
}