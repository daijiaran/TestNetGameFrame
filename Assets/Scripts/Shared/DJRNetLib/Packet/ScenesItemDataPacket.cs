using System.IO;

namespace Shared.DJRNetLib.Packet
{
    public class ScenesItemDataPacket : PacketBase
    {
        public string ItemName;
        public int ItemIndex;

        public float X;
        public float Y;
        public float Z;

        public float R_x;
        public float R_y;
        public float R_z;

        public bool isDestroy;

        /// <summary>
        /// 根据场景物体的名称、编号、位置和旋转数据构造同步数据包。
        /// </summary>
        public ScenesItemDataPacket(string itemName, int itemIndex, float x, float y, float z, float r_x, float r_y, float r_z)
        {
            ItemName = itemName;
            ItemIndex = itemIndex;
            X = x;
            Y = y;
            Z = z;
            R_x = r_x;
            R_y = r_y;
            R_z = r_z;
        }

        /// <summary>
        /// 从二进制读取器中反序列化场景物体同步数据包。
        /// </summary>
        public ScenesItemDataPacket(BinaryReader reader)
        {
            ItemName = reader.ReadString();
            ItemIndex = reader.ReadInt32();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            R_x = reader.ReadSingle();
            R_y = reader.ReadSingle();
            R_z = reader.ReadSingle();
            isDestroy = reader.ReadBoolean();
        }

        /// <summary>
        /// 创建一个空的场景物体同步数据包实例。
        /// </summary>
        public ScenesItemDataPacket() { }

        /// <summary>
        /// 将场景物体同步数据包序列化为字节数组。
        /// </summary>
        public override byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)PacketType.ScenesItem);
                writer.Write(ItemName);
                writer.Write(ItemIndex);
                writer.Write(X);
                writer.Write(Y);
                writer.Write(Z);
                writer.Write(R_x);
                writer.Write(R_y);
                writer.Write(R_z);
                writer.Write(isDestroy);
                return ms.ToArray();
            }
        }
    }
}
