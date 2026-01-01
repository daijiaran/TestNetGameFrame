using System.Collections.Generic;
using System.Net;
public class PlayersData
{
    // 字典：IP地址字符串 -> 玩家数据包
    public Dictionary<string, UserPositionPacket> Players = new Dictionary<string, UserPositionPacket>();
    
    // 【新增】字典：IP地址字符串 -> 客户端的 EndPoint (用于发送数据)
    public Dictionary<string, EndPoint> ClientEndPoints = new Dictionary<string, EndPoint>();
}