using System.IO;

namespace Shared.DJRNetLib.Packet
{
    public class UserMovePacket : PacketBase
    {
        public float H;
        public float V;

        public UserMovePacket(float h, float v)
        {
            H = h;
            V = v;
        }

        // 序列化
        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)PacketType.Move); // 写入包头
                writer.Write(H);
                writer.Write(V);
                return ms.ToArray();
            }
        }

        // 反序列化构造函数
        public UserMovePacket(BinaryReader reader)
        {
            H = reader.ReadSingle();
            V = reader.ReadSingle();
        }
    }
}