using System;
using System.Collections.Generic;
using UnityEngine;
using Shared.DJRNetLib;
using  Shared;
using Shared.DJRNetLib.Packet;

public class NetworkPlayerManager : SingelBase<NetworkPlayerManager>
{
    
    
    public GameObject PlayerPrefab;
    public PlayerControl  PlayerSelf;
    public Dictionary<String, PlayerControl> players =new Dictionary<string, PlayerControl>();
    private NetConect  netConect; 
    
    
    
    
    public void Awake()
    {
        
        netConect = ClientRoot.Instance.netConect;
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
        
        //为同步场景中玩家对象事件注册方法
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
    // public void synchronousOtherPlayer(String IpDetail , UserPositionAndStatusPacket userPositionAndStatusPacket)
    // {
    //     
    //     if (!players.ContainsKey(IpDetail))
    //     {   
    //         // 判断是否是自己
    //         // 注意：这里用名字判断可能不够严谨，最好是能对比 IP，但本地测试时 IP 可能有差异
    //         // 如果名字和自己一样，并且字典里还没存自己
    //         if (userPositionAndStatusPacket.Name == PlayerSelf.name) 
    //         {
    //             players.Add(IpDetail, PlayerSelf);
    //             Debug.Log($"成功绑定本地玩家身份: {IpDetail}");
    //         }
    //         else
    //         {
    //             PlayerControl newPlayer = CreatNewPlayer();
    //             newPlayer.PlayerName.text = userPositionAndStatusPacket.Name;
    //             players.Add(IpDetail, newPlayer);
    //         }
    //     }
    //     
    //     if (players.ContainsKey(IpDetail))
    //     {
    //             
    //             PlayerControl targetPlayer = players[IpDetail];
    //             //如果包中死亡状态为true直接执行对象死亡方法
    //             if (userPositionAndStatusPacket.isDead)
    //             {
    //                 ClientRoot.Instance.networkPlayerManager.players.Remove(IpDetail);
    //                 Debug.Log("对象死亡"+"userPositionAndStatusPacket.isDead = "+userPositionAndStatusPacket.isDead);
    //                 targetPlayer.Died();
    //                 return;
    //             }
    //             
    //             
    //             targetPlayer.PlayerName.text = userPositionAndStatusPacket.Name;
    //             
    //             Vector3 targetPos = new Vector3(userPositionAndStatusPacket.X, userPositionAndStatusPacket.Y, userPositionAndStatusPacket.Z);
    //             targetPlayer.transform.position = targetPos;
    //             Vector3 targetRot = new Vector3(userPositionAndStatusPacket.R_X, userPositionAndStatusPacket.R_Y, userPositionAndStatusPacket.R_Z);
    //             targetPlayer.transform.rotation = Quaternion.Euler(targetRot);
    //
    //             //玩家的面向方向只同步给其他玩家不同步给自己
    //             if (userPositionAndStatusPacket.Name != PlayerSelf.name)
    //             {
    //                 targetPlayer.FaceDirection.forward = new Vector3(
    //                     userPositionAndStatusPacket.Attack_X,
    //                     userPositionAndStatusPacket.Attack_Y, 
    //                     userPositionAndStatusPacket.Attack_Z
    //                     );
    //                 
    //                 Debug.Log($"更新玩家的面向方向: {userPositionAndStatusPacket.Name}");
    //             }
    //     }
    //     
    // }
    //
    
    public void synchronousOtherPlayer(String IpDetail , UserPositionAndStatusPacket userPositionAndStatusPacket)
{
    // 1. 如果字典里没有，且包没说已死，且不是自己 -> 创建新玩家
    if (!players.ContainsKey(IpDetail))
    {   
        if (userPositionAndStatusPacket.Name == PlayerSelf.name) 
        {
            // 防止重复添加自己
            if (!players.ContainsKey(IpDetail)) 
                players.Add(IpDetail, PlayerSelf);
        }
        else
        {
            // 如果这个新包已经是死亡状态，就别创建了，省得创建出来马上又销毁
            if (userPositionAndStatusPacket.isDead) return;

            PlayerControl newPlayer = CreatNewPlayer();
            newPlayer.PlayerName.text = userPositionAndStatusPacket.Name;
            players.Add(IpDetail, newPlayer);
        }
    }
    
    // 2. 如果字典里有
    if (players.ContainsKey(IpDetail))
    {
        PlayerControl targetPlayer = players[IpDetail];

        // 【核心修复】防御性判空：检查字典里的对象是否已经被外界销毁
        // Unity 重载了 == 运算符，如果是 Destroy 过的对象，这里会返回 true
        if (targetPlayer == null)
        {
            Debug.LogWarning($"发现字典中有残留的已销毁对象: {IpDetail}，正在清理...");
            players.Remove(IpDetail);
            return; // 直接返回，等待下一个包重新创建（或者忽略）
        }

        // 3. 处理死亡逻辑（服务器通知死亡）
        if (userPositionAndStatusPacket.isDead)
        {
            // 先从字典移除，防止后续逻辑再访问
            players.Remove(IpDetail);
            Debug.Log($"服务器通知对象死亡: {userPositionAndStatusPacket.Name}");
            
            // 执行死亡表现（播放动画或销毁）
            if(targetPlayer != null) 
                targetPlayer.Died();
            return;
        }
        
        // 4. 处理移动同步（只有活着的对象才同步）
        targetPlayer.PlayerName.text = userPositionAndStatusPacket.Name;
        
        Vector3 targetPos = new Vector3(userPositionAndStatusPacket.X, userPositionAndStatusPacket.Y, userPositionAndStatusPacket.Z);
        // 使用 Lerp 插值会比直接赋值更平滑
        targetPlayer.transform.position = targetPos; 

        Vector3 targetRot = new Vector3(userPositionAndStatusPacket.R_X, userPositionAndStatusPacket.R_Y, userPositionAndStatusPacket.R_Z);
        targetPlayer.transform.rotation = Quaternion.Euler(targetRot);

        if (userPositionAndStatusPacket.Name != PlayerSelf.name)
        {
            targetPlayer.FaceDirection.forward = new Vector3(
                userPositionAndStatusPacket.Attack_X,
                userPositionAndStatusPacket.Attack_Y, 
                userPositionAndStatusPacket.Attack_Z
            );
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