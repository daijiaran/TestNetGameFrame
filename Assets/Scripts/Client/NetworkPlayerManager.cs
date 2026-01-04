using System;
using System.Collections.Generic;
using UnityEngine;
using Shared.DJRNetLib;
using  Shared;
using Shared.DJRNetLib.Common;
using Shared.DJRNetLib.Packet;

public class NetworkPlayerManager : SingelBase<NetworkPlayerManager>
{
    
    
    public GameObject PlayerPrefab;
    public PlayerControl  PlayerSelf;
    public Dictionary<String, PlayerControl> players =new Dictionary<string, PlayerControl>();
    public String CurrentPlayerIP;
    private NetConect  netConect; 
    // 在类顶部定义黑名单
    private HashSet<string> deadIPs = new HashSet<string>();
    public PlayerGameData playerGameData =  new PlayerGameData();
    public bool GameReset = false;
    
    public void Awake()
    {
        
        netConect = ClientRoot.Instance.netConect;
        Init();
    }


    
    
    //开始游戏向服务器发送我要加入的信息
    public void GameStart(String name)
    {
        if (CurrentPlayerIP != null)
        {
            //如果当前是游戏重新开始则移除此玩家的黑名单
            deadIPs.Remove(CurrentPlayerIP);
        }
        
        UserJoinPacket joinPacket = new UserJoinPacket(name);
        
        joinPacket.Ip = netConect.GetLocalIpDetail();
        netConect.SendJoinMessage(joinPacket);

        PlayerSelf = CreatNewPlayer();
        playerGameData.name = name;
        PlayerSelf.name = name;
        PlayerSelf.isCurrentPlayer = true;
        
        //为同步场景中玩家对象事件注册方法
        netConect.takePlayerPacket+=synchronousOtherPlayer;
        netConect.netUserJoinEvent += Join;
    }


    public void Join(UserJoinPacket joinPacket)
    {
        if (joinPacket.Ip != null)
        {
            //移除此玩家在场景中的黑名单
            deadIPs.Remove(joinPacket.Ip);
            PlayerControl playerControl = CreatNewPlayer();
            Debug.Log("新玩家"+joinPacket.Ip+"重新加入");
            players.Add(joinPacket.Ip, playerControl);
        }
        else
        {
            Debug.Log("客户端：新玩家无法加入，因为IP是空的！！！！");
        }
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
    /// 同步服务器传来的数据
    /// </summary>
    /// <param name="IpDetail"></param>
    /// <param name="userPositionAndStatusPacket"></param>
    public void synchronousOtherPlayer(String IpDetail, UserPositionAndStatusPacket userPositionAndStatusPacket)
    {
        //如果该 IP 已经在死亡名单中，且包并不是要重置状态，直接丢弃后续所有包
        if (deadIPs.Contains(IpDetail) && !userPositionAndStatusPacket.isDead) 
        {
            return;
        }
    
        //尝试获取玩家引用
        bool exists = players.TryGetValue(IpDetail, out PlayerControl targetPlayer);
    
        // 3. 处理 Unity 对象伪空（已被 Destroy 但引用还在）
        if (exists && targetPlayer == null)
        {
            players.Remove(IpDetail);
            exists = false;
        }
    
        // 处理死亡逻辑
        if (userPositionAndStatusPacket.isDead)
        {
            if (exists && targetPlayer != null)
            {
                players.Remove(IpDetail);
                deadIPs.Add(IpDetail); // 加入黑名单，防止乱序的移动包再次触发逻辑
                targetPlayer.Died();
            }
            return;
        }
    
        // 访问 PlayerSelf 属性前必须进行严格判空
        // 这里的 PlayerSelf.name 访问是之前堆栈报错的高危点
        if (PlayerSelf == null) return; 
    
        // 6. 处理对象创建
        if (!exists)
        {
            if (userPositionAndStatusPacket.Name == PlayerSelf.name) // 安全访问
            {
                players[IpDetail] = PlayerSelf;
                targetPlayer = PlayerSelf;
            }
            else
            {
                targetPlayer = CreatNewPlayer();
                players.Add(IpDetail, targetPlayer);
            }
        }
    
        //同步数据（增加全组件判空保护）
        if (targetPlayer != null)
        {
            // 同步文本
            if (targetPlayer.PlayerName != null) 
                targetPlayer.PlayerName.text = userPositionAndStatusPacket.Name;
            
            // 同步位置旋转
            targetPlayer.transform.position = new Vector3(userPositionAndStatusPacket.X, userPositionAndStatusPacket.Y, userPositionAndStatusPacket.Z);
            targetPlayer.transform.rotation = Quaternion.Euler(userPositionAndStatusPacket.R_X, userPositionAndStatusPacket.R_Y, userPositionAndStatusPacket.R_Z);
    
            // 同步朝向（排除自己）
            if (targetPlayer != PlayerSelf && targetPlayer.FaceDirection != null)
            {
                targetPlayer.FaceDirection.forward = new Vector3(
                    userPositionAndStatusPacket.Attack_X,
                    userPositionAndStatusPacket.Attack_Y,
                    userPositionAndStatusPacket.Attack_Z
                );
            }

            if (targetPlayer == PlayerSelf)
            {
                CurrentPlayerIP = IpDetail;
            }
            
        }
    }
    
    
    
    
    public PlayerControl CreatNewPlayer()
    {
        GameObject player = Instantiate(PlayerPrefab);
        // 如果是网络玩家，一般需要禁用物理模拟，完全由位置包驱动
        player.GetComponent<Rigidbody>().isKinematic= true; 
        player.GetComponent<Collider>().enabled = true; 
        Debug.Log("新的玩家加入");
        return player.GetComponent<PlayerControl>();
    }
    
    //向客户端发送移动指令信息
    public void SendMoveToSever(UserMovePacket packet)
    {
        netConect.SendMovePacket(packet);
    }

    public void SendAttackToServer(UserAttackPacket attackPacket)
    {
        netConect.SendAttackPaket(attackPacket);
    }
    
    
    
    
}