using System.IO; // 必须引入这个，用于内存流处理

public class UserPacket
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

    //反序列化：把收到的 byte[] 变回对象
    public UserPacket(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(ms))
        {
            // 必须按写入的顺序读取！
            Name = reader.ReadString();
            
            R_X = reader.ReadSingle();
            R_Y = reader.ReadSingle();
            R_Z = reader.ReadSingle();
            
            X = reader.ReadSingle(); // ReadSingle 就是读 float
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }
    }

    // 无参构造，用于初始化
    public UserPacket() { }
}