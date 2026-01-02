using System.IO;

namespace Shared.DJRNetLib.Packet
{
    public class UserAttackPacket
    {
        public int BulltType = 1;
        public string Prefabsname;


        public UserAttackPacket(int bulltType,string prefabsname)
        {
            this.BulltType = bulltType;
            this.Prefabsname = prefabsname;
        }

        /// <summary>
        /// 序列化发送
        /// </summary>
        /// <returns></returns>
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
        /// 反序列化构造攻击包
        /// </summary>
        /// <param name="reader"></param>
        public UserAttackPacket(BinaryReader reader)
        {
            BulltType = reader.ReadInt32();
            Prefabsname = reader.ReadString();
        }
        
        
        
        public UserAttackPacket() { }
    }
}