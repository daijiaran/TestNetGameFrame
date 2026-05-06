using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class ServiceUpdate : INetworkService
{
    private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private readonly IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9050);
    private readonly byte[] buffer = new byte[1024];

    private IServerCommandHandler commandHandler;
    private IClientRegistry clientRegistry;

    /// <summary>
    /// 创建网络服务并绑定服务器监听端口。
    /// </summary>
    public ServiceUpdate()
    {
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        socket.Bind(localEndPoint);
        Console.WriteLine("服务端已启动...");
    }

    /// <summary>
    /// 初始化命令处理器与客户端注册表依赖。
    /// </summary>
    public void Initialize(IServerCommandHandler handler, IClientRegistry registry)
    {
        commandHandler = handler;
        clientRegistry = registry;
    }

    /// <summary>
    /// 供 Unity 的 Update 调用，实时接收并分发客户端发送到服务器的数据包。
    /// </summary>
    public void Update()
    {
        if (socket.Available <= 0) return;

        while (socket.Available > 0)
        {
            EndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                int receivedLength = socket.ReceiveFrom(buffer, ref remoteClient);
                string clientKey = remoteClient.ToString();

                byte[] validBytes = new byte[receivedLength];
                Array.Copy(buffer, validBytes, receivedLength);

                ParsePacket(clientKey, remoteClient, validBytes);
            }
            catch (SocketException sockEx)
            {
                if (sockEx.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Debug.Log("某个客户端强制关闭了连接 (10054)，已忽略。");
                }
                else
                {
                    Debug.Log($"Socket 错误: {sockEx.Message}");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"发生错误: {e.Message}");
            }
        }
    }

    /// <summary>
    /// 解析客户端发来的原始数据包并转交对应的业务处理逻辑。
    /// </summary>
    public void ParsePacket(string clientKey, EndPoint remoteClient, byte[] validBytes)
    {
        using (MemoryStream ms = new MemoryStream(validBytes))
        using (BinaryReader reader = new BinaryReader(ms))
        {
            byte packetTypeByte = reader.ReadByte();
            PacketType type = (PacketType)packetTypeByte;

            switch (type)
            {
                case PacketType.Join:
                    commandHandler?.HandleJoin(clientKey, remoteClient, new UserJoinPacket(reader));
                    break;
                case PacketType.Move:
                    commandHandler?.HandleMove(clientKey, new UserMovePacket(reader));
                    break;
                case PacketType.Attack:
                    commandHandler?.HandleAttack(clientKey, remoteClient, new UserAttackPacket(reader));
                    break;
            }
        }
    }

    /// <summary>
    /// 向指定客户端发送一个数据包。
    /// </summary>
    public void SendToClient(PacketBase packet, EndPoint receiverEndPoint)
    {
        socket.SendTo(ToBytes(packet), receiverEndPoint);
    }

    /// <summary>
    /// 向所有已注册客户端广播场景对象或玩家相关的数据包。
    /// </summary>
    public void SendToAllPlayerDestoryOBJ(PacketType type, PacketBase packet)
    {
        if (clientRegistry == null)
        {
            Debug.LogWarning("ClientRegistry 未初始化，广播被忽略。");
            return;
        }

        if (type == PacketType.Join && packet is UserJoinPacket joinPacket && string.IsNullOrEmpty(joinPacket.Ip))
        {
            Debug.LogError("服务端广播失败，UserJoinPacket.Ip 为空。");
            return;
        }

        foreach (var clientKvp in clientRegistry.GetAll())
        {
            try
            {
                SendToClient(packet, clientKvp.Value);
            }
            catch (Exception e)
            {
                Debug.LogError($"广播数据包失败: {e.Message}");
            }
        }
    }

    /// <summary>
    /// 将数据包对象转换为可发送的字节数组。
    /// </summary>
    private static byte[] ToBytes(PacketBase packet)
    {
        
        return packet.ToBytes();

        throw new InvalidOperationException($"Unsupported packet type: {packet.GetType().Name}");
    }
}
