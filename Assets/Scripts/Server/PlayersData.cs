using System.Collections.Generic;
using System.Net;
public class PlayersData
{
    // 字典：IP地址字符串 -> 玩家数据包
    static public Dictionary<string, UserPacket> Players = new Dictionary<string, UserPacket>();
    
    // 【新增】字典：IP地址字符串 -> 客户端的 EndPoint (用于发送数据)
    static public Dictionary<string, EndPoint> ClientEndPoints = new Dictionary<string, EndPoint>();
}