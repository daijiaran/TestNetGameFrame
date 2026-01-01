using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class ServiceUpdate
{

    /// <summary>
    /// 玩家登录事件
    /// </summary>
    public Action<string, EndPoint, UserJoinPacket> NewPlayerJoinEvent;
    
    
    
    public PlayersData playersData =  new PlayersData();
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9050);
    private byte[] buffer = new byte[1024];

    public ServiceUpdate()
    {
        socket.Bind(localEndPoint);
        Console.WriteLine("服务端已启动...");

    }
    
    /// <summary>
    /// 监听指定端口，接收客户端消息
    /// </summary>
    public void Update()
    { 
            // 1. 检查是否有待处理的数据
            // 如果没有数据 (Available == 0)，直接 return，把控制权交还给 Unity，防止卡死
            if (socket.Available <= 0) return;
            // 2. 使用 while 循环
            // 因为一帧的时间内（比如 0.016秒），可能收到了 10 个包。
            // 如果用 if，一帧只处理 1 个，会导致严重的网络延迟积压。
            while (socket.Available > 0)
            {
                EndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);
                try 
                {
                    // 因为前面检查了 Available > 0，这里的 ReceiveFrom 几乎会瞬间完成，不会卡死
                    int receivedLength = socket.ReceiveFrom(buffer, ref remoteClient);
                    string clientKey = remoteClient.ToString();
                    
                    //获取有效比特流
                    byte[] validBytes = new byte[receivedLength];
                    Array.Copy(buffer, validBytes, receivedLength);
                    
                    //解析包
                    ParsePacket(clientKey, remoteClient ,validBytes);
                }
                catch (SocketException sockEx)
                {
                    if (sockEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        UnityEngine.Debug.Log("某个客户端强迫关闭了连接 (10054)，已忽略。");
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Socket 错误: {sockEx.Message}");
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log($"发生错误: {e.Message}");
                }
            }
    }



    /// <summary>
    /// 解析包并且分发给对应的方法工作
    /// </summary>
    /// <param name="clientKey"></param>
    /// <param name="remoteClient"></param>
    /// <param name="validBytes"></param>
    public void ParsePacket(string clientKey ,EndPoint remoteClient,  byte[] validBytes)
    {
        using (MemoryStream ms = new MemoryStream(validBytes))
        using (BinaryReader reader = new BinaryReader(ms))
        {
            // 1. 读取第一个字节（包头）
            // 此时流的位置会向后移动 1 位，正好给后面的 Packet 构造函数接着读
            byte packetTypeByte = reader.ReadByte();
            PacketType type = (PacketType)packetTypeByte;


            switch (type)
            {
                case PacketType.Join:
                    UserJoinPacket userJoinPacket = new UserJoinPacket(reader);
                    NewPlayerJoin(clientKey, remoteClient, userJoinPacket);
                    break;
                case PacketType.Move:
                    UserMovePacket movePacket = new UserMovePacket(reader);
                    OnUserMove(clientKey, movePacket);
                    break;
                default:
                    break;
            }
            
            
        }
        
    }


  
    /// <summary>
    /// 新玩家注册加入
    /// </summary>
    /// <param name="clientKey"></param>
    /// <param name="remoteClient"></param>
    /// <param name="validBytes"></param>
    public void NewPlayerJoin(string clientKey ,EndPoint remoteClient,  UserJoinPacket validBytes)
    {
        // 收到 Join 包时，立刻保存客户端的 IP 地址
        if (!playersData.ClientEndPoints.ContainsKey(clientKey))
        {
            playersData.ClientEndPoints.Add(clientKey, remoteClient);
        }
        else
        {
            playersData.ClientEndPoints[clientKey] = remoteClient;
        }

        // 【修复 2】使用 ?. Invoke 防止空引用异常
        // 如果没人订阅这个事件，代码也不会报错崩溃
        NewPlayerJoinEvent?.Invoke(clientKey, remoteClient, validBytes);
    }
    
    
    
   /// <summary>
   /// 处理客户端大宋过来的移动控制数据包
   /// </summary>
   /// <param name="clientKey"></param>
   /// <param name="movePacket"></param>
    public void OnUserMove(string clientKey, UserMovePacket movePacket)
    {
        // 调用 ServerAllPlayerManager 去驱动具体的 PlayerInstance
        Server.Instance.serverAllPlayerManager.HandlePlayerMove(clientKey, movePacket);
    }
    
    
    /// <summary>
    /// 处理玩家的位置信息更新，但是目前是服务器模式并不接收玩家位置信息只
    /// </summary>
    /// <param name="clientKey"></param>
    /// <param name="remoteClient"></param>
    /// <param name="validBytes"></param>
    public void UserPositionPacketProcess(string clientKey ,EndPoint remoteClient,  UserPositionPacket validBytes)
    {
        
        UpdatePlayerData(clientKey,remoteClient,validBytes);
            
        //广播所有玩家位置信息
        SendToAllPlayer();
    }
    
    
    
    
    /// <summary>
    /// 更新玩家数据，添加对局玩家
    /// </summary>
    /// <param name="clientKey"></param>
    /// <param name="remoteClient"></param>
    /// <param name="newPositionPacket"></param>
    public void UpdatePlayerData(string clientKey , EndPoint remoteClient , UserPositionPacket newPositionPacket )
    {
        // 更新玩家数据
        if (playersData.Players.ContainsKey(clientKey))
        {
            playersData.Players[clientKey] = newPositionPacket;
        }
        else
        {
            playersData.Players.Add(clientKey, newPositionPacket);
            UnityEngine.Debug.Log($"新玩家加入: {newPositionPacket.Name}");
        }

        // 更新或保存客户端的 EndPoint
        if (!playersData.ClientEndPoints.ContainsKey(clientKey))
        {
            playersData.ClientEndPoints.Add(clientKey, remoteClient);
        }
        else
        {
            playersData.ClientEndPoints[clientKey] = remoteClient;
        }
    }
    
    
    
    
    /// <summary>
    /// 实时广播：把服务器上运行的所有玩家数据，发给所有客户端
    /// </summary>
    public void SendToAllPlayer()
    {
        // 1. 遍历每一个需要接收数据的客户端 (接收者)
        foreach (var clientKvp in playersData.ClientEndPoints)
        {
            EndPoint receiverEndPoint = clientKvp.Value;

            // 2. 遍历所有玩家的数据 (要发送的内容)
            foreach (var playerKvp in playersData.Players)
            {
                UserPositionPacket dataToSend = playerKvp.Value;
                
                // 发送！
                // 这样接收者(Receiver) 就会收到 玩家A、玩家B、玩家C... 的一个个包
                socket.SendTo(dataToSend.ToBytes(), receiverEndPoint);
            }
        }
    }
}