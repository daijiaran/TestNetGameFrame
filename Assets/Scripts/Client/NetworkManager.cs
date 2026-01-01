using System;
using System.Collections.Generic;
using UnityEngine;
using Shared.DJRNetLib;
using  Shared;
using Shared.DJRNetLib.Packet;

public class NetworkManager : SingelBase<NetworkManager>
{
    
    
    public GameObject PlayerPrefab;
    public PlayerControl  PlayerSelf;
    public Dictionary<String, PlayerControl> players;
    private NetConect  netConect; 
    
    
    
    
    public void Awake()
    {
        // 1. 初始化玩家字典（解决当前的报错）
        players = new Dictionary<string, PlayerControl>();
        // 3. 初始化网络连接对象（防止 Update 报错）
        netConect = new NetConect();
        // 4. 开启接收线程（否则收不到消息）
        netConect.ReceiveInformation();
        Init();
    }


    
    
    //开始游戏向服务器发送我要加入的信息
    public void GameStart(String name)
    {
        UserJoinPacket joinPacket = new UserJoinPacket(name);
        joinPacket.Ip = netConect.GetLocalIpDetail();
        netConect.SendJoinMessage(joinPacket);

        PlayerSelf = CreatNewPlayer();
        PlayerSelf.name = name;
        PlayerSelf.isCurrentPlayer = true;
        
        // 【修改】获取本地IPKey并绑定
        string myKey = netConect.GetLocalIpDetail();
        if (!string.IsNullOrEmpty(myKey))
        {
            // 防止重复添加
            if (!players.ContainsKey(myKey))
            {
                players.Add(myKey, PlayerSelf);
            }
        }
        
        
        
        netConect.takePlayerPacket+=synchronousOtherPlayer;
    }
    
    

    private void Update()
    {
        
        //驱动 NetConect 处理消息队列
        if (netConect != null)
        {
            netConect.Update();
        }
    }
    
    
    
    
    
    /// <summary>
    /// 同步服务器上的数据
    /// </summary>
    /// <param name="IpDetail"></param>
    /// <param name="userPositionPacket"></param>
    public void synchronousOtherPlayer(String IpDetail , UserPositionPacket userPositionPacket)
    {
        if (!players.ContainsKey(IpDetail))
        {   
            //如果没有这个玩家就创建一个新的玩家并且配置上位置信息
            players.Add(IpDetail,CreatNewPlayer());
            players[IpDetail].PlayerName.text = userPositionPacket.Name;
            
            // 同步旋转
            // 将接收到的 X, Y, Z 欧拉角转换为 Quaternion 旋转
            Vector3 currentRot = new Vector3(userPositionPacket.R_X, userPositionPacket.R_Y, userPositionPacket.R_Z);
            players[IpDetail].transform.rotation = Quaternion.Euler(currentRot);
            
            Vector3 vtr3 = new Vector3(userPositionPacket.X,userPositionPacket.Y,userPositionPacket.Z);
            players[IpDetail].transform.position = vtr3;
        }
        else
        {
            //如果此玩家存在则直接同步位置信息
            players[IpDetail].PlayerName.text = userPositionPacket.Name;
            // 同步旋转
            // 将接收到的 X, Y, Z 欧拉角转换为 Quaternion 旋转
            Vector3 currentRot = new Vector3(userPositionPacket.R_X, userPositionPacket.R_Y, userPositionPacket.R_Z);
            players[IpDetail].transform.rotation = Quaternion.Euler(currentRot);
            
            Vector3 vtr3 = new Vector3(userPositionPacket.X,userPositionPacket.Y,userPositionPacket.Z);
            players[IpDetail].transform.position = vtr3;
        }
    }

    
    
    

    /// <summary>
    /// 在场景里面玩家
    /// </summary>
    /// <returns></returns>
    public PlayerControl CreatNewPlayer()
    {
        GameObject player = Instantiate(PlayerPrefab);
        player.GetComponent<Rigidbody>().isKinematic= true;
        player.GetComponent<Collider>().enabled = false;
        Debug.Log("新的玩家加入");
        return player.GetComponent<PlayerControl>();
    }
    
    //向客户端发送移动指令信息
    public void SendMoveToSever(UserMovePacket packet)
    {
        netConect.SendMovePacket(packet);
    }
    
    
    
    
}