using System.IO;

namespace Shared.DJRNetLib.Packet
{
    public class UserAttackPacket
    {
        public int BulltType = 1;
        public string Prefabsname;

        /// <summary>
        /// 根据子弹类型与预制体名称构造攻击数据包。
        /// </summary>
        public UserAttackPacket(int bulltType, string prefabsname)
        {
            BulltType = bulltType;
            Prefabsname = prefabsname;
        }

        /// <summary>
        /// 将攻击数据包序列化为字节数组。
        /// </summary>
        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)PacketType.Attack);
                writer.Write(BulltType);
                writer.Write(Prefabsname);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 从二进制读取器中反序列化攻击数据包。
        /// </summary>
        public UserAttackPacket(BinaryReader reader)
        {
            BulltType = reader.ReadInt32();
            Prefabsname = reader.ReadString();
        }

        /// <summary>
        /// 创建一个空的攻击数据包实例。
        /// </summary>
        public UserAttackPacket() { }
    }
}
