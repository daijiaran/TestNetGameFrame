using System;
using Unity.VisualScripting;
using UnityEngine;
using Shared.DJRNetLib.Packet; // 引用包定义
using System.Net;
using Shared.DJRNetLib;

public class Server : SingelBase<Server>
{
    public ServiceUpdate serviceUpdate;
    public ServerAllPlayerManager serverAllPlayerManager;

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        Debug.Log("服务器开始运行！！！");        
        
        serviceUpdate = new ServiceUpdate();
        transform.AddComponent<ServerAllPlayerManager>();
        serverAllPlayerManager = transform.GetComponent<ServerAllPlayerManager>();

        //订阅玩家加入事件
        if (serviceUpdate != null)
        {
            serviceUpdate.NewPlayerJoinEvent += OnPlayerJoin;
        }
    }

    
    
    private void OnPlayerJoin(string clientKey, EndPoint remoteEndPoint, UserJoinPacket joinPacket)
    {
        Debug.Log($"接收到玩家加入请求: {clientKey}");
        serverAllPlayerManager.CreatePlayerInstance(clientKey,joinPacket); 
    }

    
    
    
    
    void Update()
    {
        if (serverAllPlayerManager != null && serviceUpdate != null)
        {
            // 将逻辑层的数据同步给网络层用于广播
            serviceUpdate.playersData.Players = serverAllPlayerManager.AllPlayerInstancesUserPositionPackets;
            serviceUpdate.Update();
            serviceUpdate.SendToAllPlayer();
        }
    }
    
    
    
    
    
    // 在销毁时取消订阅防止内存泄漏
    private void OnDestroy()
    {
        if (serviceUpdate != null)
        {
            serviceUpdate.NewPlayerJoinEvent -= OnPlayerJoin;
        }
    }
}