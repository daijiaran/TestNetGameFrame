using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Shared.DJRNetLib;

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
            
            //持续向所有玩家广播服务器场景中的玩家的位置
            SendToAllPlayer();
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
                case PacketType.Position:
                    UserPositionPacket userPositionPacket = new UserPositionPacket(reader);
                    UserPositionPacketProcess(clientKey, remoteClient, userPositionPacket);
                    break;
                case PacketType.Move:
                    break;
                default:
                    break;
            }
            
            
        }
        
    }


    /// <summary>
    /// 新玩家注册
    /// </summary>
    /// <param name="clientKey"></param>
    /// <param name="remoteClient"></param>
    /// <param name="validBytes"></param>
    public void NewPlayerJoin(string clientKey ,EndPoint remoteClient,  UserJoinPacket validBytes)
    {
        NewPlayerJoinEvent.Invoke(clientKey, remoteClient, validBytes);
    }
    
    
    
    
    
    /// <summary>
    /// 处理玩家的位置信息更新
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
    /// 实时广播：把服务器上运行的玩家的数据其他所有客户端
    /// </summary>
    public void SendToAllPlayer()
    {
        
        foreach (var kvp in playersData.ClientEndPoints)
        {
            string targetKey = kvp.Key;
            EndPoint targetEndPoint = kvp.Value;
            
            socket.SendTo(playersData.Players[targetKey].ToBytes(), targetEndPoint);
        }
    }
}