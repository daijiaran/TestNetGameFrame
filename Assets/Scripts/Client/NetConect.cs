using System;
using System.Collections.Concurrent;
using System.IO; // 【1】必须引入这个命名空间
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;


public class NetConect
    {
        

        
        
        // 接受到信息的时候通知 UI 更新
        public Action<String> takeMessage;
        //发送失败的时候
        public Action<String> errCallback;
        
        
        //分别接受其他玩家的IP地址，和玩家的包
        public Action<String,UserPositionPacket> takePlayerPacket;
        
        //实现网络信息发送的管子
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        
        // 目标IP地址
        private IPEndPoint ServerAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        // 【2】创建一个线程安全的队列，作为“篮子”
        private ConcurrentQueue<string> _msgQueue = new ConcurrentQueue<string>();

        // 【新增】专门存放玩家数据包的队列
        private ConcurrentQueue<PacketInfo> _packetQueue = new ConcurrentQueue<PacketInfo>();
        
        
        
        
        
        // 构造函数
        public NetConect()
        {
            // 【新增】这一行非常关键！
            // 绑定到本机任意可用端口（IPAddress.Any, 0）
            // 只有绑定了，Socket 才知道要“打开耳朵”听数据
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        }
        
        // 在 NetConect 类中添加
        public string GetLocalIpDetail()
        {
            if (socket != null && socket.LocalEndPoint != null)
            {
                return socket.LocalEndPoint.ToString();
            }
            return "";
        }
        
        
        
        
        
        
        
        
        
        
        
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            // 这里建议加 try-catch 防止没连网报错
            try 
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                socket.SendTo(data, ServerAddress);
            }
            catch (Exception e)
            {
                errCallback.Invoke(e.Message);
            }
        }
        
        
        
        
        /// <summary>
        /// 发送加入游戏的信息
        /// </summary>
        /// <param name="message"></param>
        public void SendJoinMessage(UserJoinPacket joinPacket)
        {
            // 这里建议加 try-catch 防止没连网报错
            try 
            {
                byte[] data = joinPacket.Tobyte();
                socket.SendTo(data, ServerAddress);
            }
            catch (Exception e)
            {
                errCallback.Invoke(e.Message);
            }
        }
        
        
        
        /// <summary>
        /// 发送当前客户端的移动指令包
        /// </summary>
        /// <param name="movePacket"></param>
        public void SendMovePacket(UserMovePacket movePacket)
        {
            try 
            {
                byte[] data = movePacket.ToBytes();
                socket.SendTo(data, ServerAddress);
            }
            catch (Exception e)
            {
                errCallback.Invoke(e.Message);
            }
        }


        

        /// <summary>
        /// 发送位置信息
        /// </summary>
        /// <param name="positionPacket"></param>
        public void SendPositionPacket(UserPositionPacket positionPacket)
        {
            // 【关键】调用 ToBytes() 变成数组，再发送
            byte[] data = positionPacket.ToBytes();
            
            // 这里建议加 try-catch 防止没连网报错
            try 
            {
                socket.SendTo(data, ServerAddress);
            }
            catch (Exception e)
            {
                errCallback.Invoke(e.Message);
            }
        }

        

        
        
        /// <summary>
        /// 线程方法，开启一个监听服务器信息的线程
        /// </summary>
        public void ReceiveInformation()
        {
            Task.Run(() => 
            {
                byte[] recvBuffer = new byte[1024];
                while (true)
                {
                    try
                    {
                        EndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
                        
                        // 【修改】实现接收逻辑
                        int length = socket.ReceiveFrom(recvBuffer, ref remotePoint);
                        
                        // 截取有效数据
                        byte[] validBytes = new byte[length];
                        Array.Copy(recvBuffer, validBytes, length);

                        ParsePacket(validBytes);
                        
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            });
        }

        
        
        /// <summary>
        /// 解析包并且分发给对应的方法工作
        /// </summary>
        /// <param name="remoteClient"></param>
        /// <param name="validBytes"></param>
        public void ParsePacket(byte[] validBytes)
        {
            using (MemoryStream ms = new MemoryStream(validBytes))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                // 1. 读取第一个字节（包头）
                // 此时流的位置会向后移动 1 位，正好给后面的 Packet 构造函数接着读
                byte packetTypeByte = reader.ReadByte();
                PacketType type = (PacketType)packetTypeByte;


                switch (type)
                {
                    case PacketType.Position:
                        UserPositionPacket userPositionPacket = new UserPositionPacket(reader);
                        UserPositionPacketProcess(userPositionPacket);
                        break;
                    default:
                        break;
                }
            }
        
        }
        
        /// <summary>
        /// 处理玩家的位置信息更新
        /// </summary>
        /// <param name="remotePoint"></param>
        /// <param name="validBytes"></param>
        public void UserPositionPacketProcess(UserPositionPacket validBytes)
        {
            // 【新增】放入队列等待 Unity 主线程处理
            _packetQueue.Enqueue(new PacketInfo() { 
                Ip = validBytes.Ip, 
                PositionPacket = validBytes
            });
        }
        
        
        /// <summary>
        /// 供 Unity 的 Update 调用实时向服务器发送信息
        /// </summary>
        public void Update()
        {
            while (_msgQueue.TryDequeue(out string msg))
            {
                takeMessage?.Invoke(msg);
            }

            //处理玩家数据包队列
            while (_packetQueue.TryDequeue(out PacketInfo info))
            {
                // 触发事件，将 IP 和 包数据传给 NetworkManager
                takePlayerPacket?.Invoke(info.Ip, info.PositionPacket);
            }
        }

        
        
        
        public void Close()
        {
            socket.Close();
        }
    }
