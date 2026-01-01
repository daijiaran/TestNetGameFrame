using System.IO;

namespace Shared.DJRNetLib
{
    public class UserJoinPacket : PacketBase
    {
        public string name;

        public byte[] Tobyte()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                //写入消息头
                writer.Write((byte)PacketType.Join);
                
                //写入消息体
                writer.Write(name);
                return ms.ToArray(); // 返回生成的字节数组
            }
        }
        
        /// <summary>
        /// 构造函数，构造玩家加入包
        /// </summary>
        /// <param name="reader"></param>
        public UserJoinPacket(BinaryReader reader)
        {
            name=reader.ReadString();
        }

        public UserJoinPacket(string name)
        {
            this.name = name;
        }
        
        public UserJoinPacket() {}
    }
}