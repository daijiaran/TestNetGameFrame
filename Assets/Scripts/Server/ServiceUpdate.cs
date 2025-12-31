using System;
using System.Net;
using System.Net.Sockets;


public class ServiceUpdate
{
    private PlayersData playersData =  new PlayersData();
    public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    public IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9050);
    public byte[] buffer = new byte[1024];

    public ServiceUpdate()
    {
        socket.Bind(localEndPoint);
        Console.WriteLine("服务端已启动...");

    }
    
    public void Update()
    { 
        
        EndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);
        
        // 接收数据
        int receivedLength = socket.ReceiveFrom(buffer, ref remoteClient);
        string clientKey = remoteClient.ToString();
    
        try 
        {
            // 截取有效数据
            byte[] validBytes = new byte[receivedLength];
            Array.Copy(buffer, validBytes, receivedLength);
            
            
            
            
            // 反序列化
            UserPacket newPacket = new UserPacket(validBytes);
            Console.WriteLine($"收到 {clientKey} ({newPacket.Name}) 的位置: {newPacket.X:F2}, {newPacket.Y:F2}, {newPacket.Z:F2}");
            
            
            
            
            
            // 更新玩家数据
            if (PlayersData.Players.ContainsKey(clientKey))
            {
                PlayersData.Players[clientKey] = newPacket;
            }
            else
            {
                PlayersData.Players.Add(clientKey, newPacket);
                Console.WriteLine($"新玩家加入: {newPacket.Name}");
            }
    
            
            
            // 【新增】更新或保存客户端的 EndPoint
            if (!PlayersData.ClientEndPoints.ContainsKey(clientKey))
            {
                PlayersData.ClientEndPoints.Add(clientKey, remoteClient);
            }
            else
            {
                PlayersData.ClientEndPoints[clientKey] = remoteClient;
            }
            
            
    
            SendToAllPlayer(clientKey, validBytes);
    
        }
        catch (SocketException sockEx)
        {
            // 专门捕获 Socket 错误
            if (sockEx.SocketErrorCode == SocketError.ConnectionReset)
            {
                Console.WriteLine("某个客户端强迫关闭了连接 (10054)，已忽略。");
            }
            else
            {
                Console.WriteLine($"Socket 错误: {sockEx.Message}");
            }
        }
        catch (Exception e)
        {
            // 捕获其他所有错误（如反序列化失败等）
            Console.WriteLine($"发生错误: {e.Message}");
        }
        
    }
    
    /// <summary>
    /// 广播：把收到的这个包发给其他所有客户端
    /// </summary>
    /// <param name="clientKey"></param>
    /// <param name="data"></param>
    public void SendToAllPlayer(string clientKey , byte[] data)
    {
        
        foreach (var kvp in PlayersData.ClientEndPoints)
        {
            string targetKey = kvp.Key;
            EndPoint targetEndPoint = kvp.Value;

            
            //不发给自己 (可选，通常客户端有本地预测，不需要服务器发回给自己)
            if (targetKey != clientKey)
            {
                socket.SendTo(data, targetEndPoint);
            }
        }
    }
}