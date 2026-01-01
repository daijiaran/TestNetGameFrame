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
    public void synchronousOtherPlayer(String IpDetail , UserPositionPacket userPositionPacket)
    {
        // ------------------ 【修正逻辑开始】 ------------------
        
        // 1. 如果字典里没有这个玩家，说明是新玩家（或者还没把自己加进去）
        if (!players.ContainsKey(IpDetail))
        {   
            // 判断是否是自己
            // 注意：这里用名字判断可能不够严谨，最好是能对比 IP，但本地测试时 IP 可能有差异
            // 如果名字和自己一样，并且字典里还没存自己
            if (userPositionPacket.Name == PlayerSelf.name) 
            {
                // 把自己加入字典，与这个 IP 绑定
                players.Add(IpDetail, PlayerSelf);
                Debug.Log($"成功绑定本地玩家身份: {IpDetail}");
            }
            else
            {
                // 是其他玩家，实例化一个新的
                PlayerControl newPlayer = CreatNewPlayer();
                newPlayer.PlayerName.text = userPositionPacket.Name;
                players.Add(IpDetail, newPlayer);
            }
        }
        
        if (players.ContainsKey(IpDetail))
        {
            PlayerControl targetPlayer = players[IpDetail];
            
                Vector3 targetPos = new Vector3(userPositionPacket.X, userPositionPacket.Y, userPositionPacket.Z);
                targetPlayer.transform.position = targetPos;
                // 同步旋转
                Vector3 targetRot = new Vector3(userPositionPacket.R_X, userPositionPacket.R_Y, userPositionPacket.R_Z);
                targetPlayer.transform.rotation = Quaternion.Euler(targetRot);
        }
        
    }
    
    

    public PlayerControl CreatNewPlayer()
    {
        GameObject player = Instantiate(PlayerPrefab);
        // 如果是网络玩家，一般需要禁用物理模拟，完全由位置包驱动
        player.GetComponent<Rigidbody>().isKinematic= true; 
        player.GetComponent<Collider>().enabled = false; // 视需求而定，可能需要保留 Collider 做检测
        Debug.Log("新的玩家加入");
        return player.GetComponent<PlayerControl>();
    }
    
    //向客户端发送移动指令信息
    public void SendMoveToSever(UserMovePacket packet)
    {
        netConect.SendMovePacket(packet);
    }
    
    
    
    
}