using System.IO;
using Shared.DJRNetLib; // 必须引入这个，用于内存流处理

public class UserPositionPacket
{
    public string Name;
    public float R_X, R_Y, R_Z;
    public float X, Y, Z; // 替代 NetVector3，方便理解底层

    //序列化：把这个对象变成 byte[]，方便发送
    public byte[] ToBytes()
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            //写入消息头
            writer.Write((byte)PacketType.Position);
            
            //写入消息体
            writer.Write(Name); // 写入名字字符串
            writer.Write(R_X);
            writer.Write(R_Y);
            writer.Write(R_Z);
            
            writer.Write(X);    // 写入坐标 X
            writer.Write(Y);    // 写入坐标 Y
            writer.Write(Z);    // 写入坐标 Z
            return ms.ToArray(); // 返回生成的字节数组
        }
    }

    
    
    /// <summary>
    /// 构造函数，专门用来接收 BinaryReader
    /// 构造一个UserPosition对象
    /// </summary>
    /// <param name="reader"></param>
    public UserPositionPacket(BinaryReader reader)
    {
        // 直接从传进来的 reader 里接着往下读
        // 注意：顺序必须和 ToBytes() 里的消息体的写入顺序完全一致！
        Name = reader.ReadString();
        
        R_X = reader.ReadSingle();
        R_Y = reader.ReadSingle();
        R_Z = reader.ReadSingle();
        
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
    }
    

    // 无参构造，用于初始化
    public UserPositionPacket() { }
}