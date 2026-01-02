using System.IO;

namespace Shared.DJRNetLib.Packet
{
    public class ScenesItemDataPacket:PacketBase
    {
        //用于生成对象
        public string ItemName;
        //用于查询对象
        public int ItemIndex;
        
        public float X;
        public float Y;
        public float Z;
        
        public float R_x;
        public float R_y;
        public float R_z;
        
        public bool isDestroy;


        
        public ScenesItemDataPacket(string itemName, int  itemIndex,float x, float y, float z, float r_x, float r_y, float r_z)
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
        
        //反序列化
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
        
        
        
        public byte[] Tobytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                //写入头
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

        public ScenesItemDataPacket() { }
        
    }
}