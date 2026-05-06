using System.IO;

namespace Shared.DJRNetLib.Packet
{
    public class UserMovePacket : PacketBase
    {
        public float H;
        public float V;

        public float D_x;
        public float D_y;
        public float D_z;

        public float Attack_x;
        public float Attack_y;
        public float Attack_z;

        /// <summary>
        /// 根据移动输入和朝向数据构造移动同步数据包。
        /// </summary>
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

        /// <summary>
        /// 将移动同步数据包序列化为字节数组。
        /// </summary>
        public override byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)PacketType.Move);
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

        /// <summary>
        /// 从二进制读取器中反序列化移动同步数据包。
        /// </summary>
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
