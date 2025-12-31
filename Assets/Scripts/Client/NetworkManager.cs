using System;
using System.Collections.Generic;
using UnityEngine;
using Shared.DJRNetLib;
using  Shared;

public class NetworkManager : SingelBase<NetworkManager>
{
    
    
    public GameObject PlayerPrefab;
    public PlayerControl playerSelf;
    
    public Dictionary<String, PlayerControl> players;
    
    public bool isJoinServise = false;
    
    private UserPacket userPacket;
    private NetConect  netConect; 
    
    
    
    
    public void Awake()
    {
        // 1. 初始化玩家字典（解决当前的报错）
        players = new Dictionary<string, PlayerControl>();

        // 2. 初始化数据包对象（防止 SendPlayer 报错）
        userPacket = new UserPacket();

        // 3. 初始化网络连接对象（防止 Update 报错）
        netConect = new NetConect();
        
        
        
        // 4. 开启接收线程（否则收不到消息）
        netConect.ReceiveInformation();
        Init();
    }


    
    
    //先创建自己的玩家
    public void GameStart(String name)
    {
        GameObject player = Instantiate(PlayerPrefab);
        playerSelf = player.GetComponent<PlayerControl>();
        playerSelf.PlayerName.text = name;
        playerSelf.isCurrentPlayer = true;
        isJoinServise = true;
        
        netConect.takePlayerPacket+=synchronousOtherPlayer;
    }

    private void Update()
    {
        
        //驱动 NetConect 处理消息队列
        if (netConect != null)
        {
            netConect.Update();
        }
        
        if (isJoinServise)
        {
           SendPlayer();
        }
    }




    public void SendPlayer()
    {
        // 确保 userPacket 不为空
        if (userPacket == null) userPacket = new UserPacket();
        
        userPacket.Name  = playerSelf.PlayerName.text; 
        
        userPacket.R_X = playerSelf.transform.rotation.eulerAngles.x;
        userPacket.R_Y = playerSelf.transform.rotation.eulerAngles.y;
        userPacket.R_Z = playerSelf.transform.rotation.eulerAngles.z;
        
        userPacket.X = playerSelf.transform.position.x;
        userPacket.Y = playerSelf.transform.position.y;
        userPacket.Z = playerSelf.transform.position.z;
        netConect.SendPositionPacket(userPacket);
    }

    public void synchronousOtherPlayer(String IpDetail , UserPacket userPacket)
    {
        
        
        if (!players.ContainsKey(IpDetail))
        {   
            //如果没有这个玩家就创建一个新的玩家并且配置上位置信息
            players.Add(IpDetail,CreatNewPlayer());
            players[IpDetail].PlayerName.text = userPacket.Name;
            
            // 同步旋转
            // 将接收到的 X, Y, Z 欧拉角转换为 Quaternion 旋转
            Vector3 currentRot = new Vector3(userPacket.R_X, userPacket.R_Y, userPacket.R_Z);
            players[IpDetail].transform.rotation = Quaternion.Euler(currentRot);
            
            Vector3 vtr3 = new Vector3(userPacket.X,userPacket.Y,userPacket.Z);
            players[IpDetail].transform.position = vtr3;
        }
        else
        {
            //如果此玩家存在则直接同步位置信息
            players[IpDetail].PlayerName.text = userPacket.Name;
            
            // 同步旋转
            // 将接收到的 X, Y, Z 欧拉角转换为 Quaternion 旋转
            Vector3 currentRot = new Vector3(userPacket.R_X, userPacket.R_Y, userPacket.R_Z);
            players[IpDetail].transform.rotation = Quaternion.Euler(currentRot);
            
            Vector3 vtr3 = new Vector3(userPacket.X,userPacket.Y,userPacket.Z);
            players[IpDetail].transform.position = vtr3;
        }
    }


    public PlayerControl CreatNewPlayer()
    {
        GameObject player = Instantiate(PlayerPrefab);
        player.GetComponent<Rigidbody>().isKinematic= true;
        player.GetComponent<Collider>().enabled = false;
        Debug.Log("新的玩家加入");
        return player.GetComponent<PlayerControl>();
    }
    
    
    
    
    
    
}