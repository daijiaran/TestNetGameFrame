using UnityEngine;

public class Server : SingelBase<Server>
{
    public ServiceUpdate serviceUpdate;
    public ServerAllPlayerManager serverAllPlayerManager;
    void Start()
    {
        Debug.Log("服务器开始运行！！！");        
        GameObject serverAllPlayerManagerOBJ = Instantiate(Resources.Load<GameObject>("Prefabs/ServerAllPlayerManager"));
        serverAllPlayerManager = serverAllPlayerManagerOBJ.GetComponent<ServerAllPlayerManager>();
        serviceUpdate = new ServiceUpdate();
    }

    void Update()
    {
        //同步当前服务器上的数据
        serviceUpdate.playersData.Players = serverAllPlayerManager.AllPlayerInstancesUserPositionPackets;
        serviceUpdate.Update();
    }
}
