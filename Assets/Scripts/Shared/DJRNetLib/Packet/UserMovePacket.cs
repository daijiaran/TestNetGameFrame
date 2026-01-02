using System.IO;
using System.Numerics;

namespace Shared.DJRNetLib.Packet
{
    public class UserMovePacket : PacketBase
    {
        //移动的时候的XZ轴上的输入
        public float H;
        public float V;
        
        //direaction移动的方向
        public  float D_x;
        public  float D_y;
        public  float D_z;
        //枪口的方向
        public float Attack_x;
        public float Attack_y;
        public float Attack_z;

        public UserMovePacket(float h, float v, float d_x, float d_y, float d_z)
        {
            H = h;
            V = v;
            D_x = d_x;
            D_y = d_y;
            D_z = d_z;
            Attack_x = D_x;
            Attack_y = D_y;
            Attack_z = D_z;
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
                
                writer.Write(D_x);
                writer.Write(D_y);
                writer.Write(D_z);
                
                writer.Write(Attack_x);
                writer.Write(Attack_y);
                writer.Write(Attack_z);
                
                
                return ms.ToArray();
            }
        }

        // 反序列化构造函数
        public UserMovePacket(BinaryReader reader)
        {
            H = reader.ReadSingle();
            V = reader.ReadSingle();
            
            D_x = reader.ReadSingle();
            D_y = reader.ReadSingle();
            D_z = reader.ReadSingle();
            
            Attack_x = reader.ReadSingle();
            Attack_y = reader.ReadSingle();
            Attack_z = reader.ReadSingle();
        }
    }
}