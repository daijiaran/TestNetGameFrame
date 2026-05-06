using System.IO;

namespace Shared.DJRNetLib.Packet
{
    public class UserPositionAndStatusPacket : PacketBase
    {
        public string Name;
        public float R_X;
        public float R_Y;
        public float R_Z;
        public float X;
        public float Y;
        public float Z;
        public float Attack_X;
        public float Attack_Y;
        public float Attack_Z;
        public float Health;
        public bool isDead;

        /// <summary>
        /// 将玩家位置、朝向和状态数据序列化为字节数组。
        /// </summary>
        public override byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)PacketType.PositionAndStatus);
                writer.Write(Name);
                writer.Write(R_X);
                writer.Write(R_Y);
                writer.Write(R_Z);
                writer.Write(X);
                writer.Write(Y);
                writer.Write(Z);
                writer.Write(Attack_X);
                writer.Write(Attack_Y);
                writer.Write(Attack_Z);
                writer.Write(Ip);
                writer.Write(Health);
                writer.Write(isDead);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 从二进制读取器中反序列化玩家位置与状态数据包。
        /// </summary>
        public UserPositionAndStatusPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
            R_X = reader.ReadSingle();
            R_Y = reader.ReadSingle();
            R_Z = reader.ReadSingle();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            Attack_X = reader.ReadSingle();
            Attack_Y = reader.ReadSingle();
            Attack_Z = reader.ReadSingle();
            Ip = reader.ReadString();
            Health = reader.ReadSingle();
            isDead = reader.ReadBoolean();
        }

        /// <summary>
        /// 创建一个空的玩家位置与状态数据包实例。
        /// </summary>
        public UserPositionAndStatusPacket() { }
    }
}
