using System.Collections.Generic;
using System.Net;
using Shared.DJRNetLib.Packet;

public class PlayersData
{
    // 字典：IP地址字符串 -> 玩家数据包
    public Dictionary<string, UserPositionAndStatusPacket> Players = new Dictionary<string, UserPositionAndStatusPacket>();
    
    // P地址字符串 -> 客户端的 EndPoint (用于发送数据)
    public Dictionary<string, EndPoint> ClientEndPoints = new Dictionary<string, EndPoint>();
}