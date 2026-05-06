using System.IO;
using Shared.DJRNetLib.Packet;

namespace Shared.DJRNetLib
{
    public class UserJoinPacket : PacketBase
    {
        public string name;

        /// <summary>
        /// 从二进制读取器中反序列化玩家加入数据包。
        /// </summary>
        public UserJoinPacket(BinaryReader reader)
        {
            name = reader.ReadString();
            Ip = reader.ReadString();
        }

        /// <summary>
        /// 根据玩家名称构造加入游戏数据包。
        /// </summary>
        public UserJoinPacket(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 创建一个空的玩家加入数据包实例。
        /// </summary>
        public UserJoinPacket() { }

        /// <summary>
        /// 将玩家加入数据包序列化为字节数组。
        /// </summary>
        public override byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)PacketType.Join);
                writer.Write(name);
                writer.Write(Ip);
                return ms.ToArray();
            }
        }
    }
}
