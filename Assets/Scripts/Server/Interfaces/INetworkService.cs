using System.Net;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;

public interface INetworkService
{
    /// <summary>
    /// 向指定客户端发送一个数据包。
    /// </summary>
    void SendToClient(PacketBase packet, EndPoint receiverEndPoint);

    /// <summary>
    /// 向所有客户端广播需要同步的对象数据包。
    /// </summary>
    void SendToAllPlayerDestoryOBJ(PacketType type, PacketBase packet);
}
