namespace Shared.DJRNetLib
{
    public enum PacketType : byte
    {
        None = 0,
        Join = 1,
        PositionAndStatus = 2,
        Move = 3,
        Attack = 4,
        ScenesItem = 5,
    }
}