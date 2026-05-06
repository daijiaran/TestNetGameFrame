namespace Shared.DJRNetLib.Packet
{
    public abstract class PacketBase
    {
        public string Ip;

        /// <summary>
        /// 将当前数据包序列化为可传输的字节数组。
        /// </summary>
        public abstract byte[] ToBytes();
    }
}
