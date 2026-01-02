using System;
using System.Collections.Concurrent;
using System.IO; // 【1】必须引入这个命名空间
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using UnityEngine;


public class NetConect
    {
        

        
        
        // 接受到信息的时候通知 UI 更新
        public Action<String> takeMessage;
        //发送失败的时候
        public Action<String> errCallback;
        
        
        //分别接收并同步其他玩家的IP地址，和玩家的包
        public Action<String,UserPositionAndStatusPacket> takePlayerPacket;
        //分别接收并同步场景中所有物体的数据
        public Action<ScenesItemDataPacket> takeSceneItemPacket;

        
        //实现网络信息发送的管子
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        
        // 目标IP地址
        private IPEndPoint ServerAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        // 专门存放玩家聊天信息的队列
        private ConcurrentQueue<string> _msgQueue = new ConcurrentQueue<string>();

        // 专门存放玩家数据包的队列
        private ConcurrentQueue<PacketInfo> _packetQueue = new ConcurrentQueue<PacketInfo>();
        
        // 专门存放场景数据变化包的队列
        private ConcurrentQueue<ScenesItemDataPacket> _ScenesIteamDataQueue = new ConcurrentQueue<ScenesItemDataPacket>();

        
        
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
        /// 发送攻击指令
        /// </summary>
        /// <param name="attackPacket"></param>
        public void SendAttackPaket(UserAttackPacket attackPacket)
        {
            try 
            {
                byte[] data = attackPacket.ToBytes();
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
        public void SendPositionPacket(UserPositionAndStatusPacket positionPacket)
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
                
                        // 接收数据
                        int length = socket.ReceiveFrom(recvBuffer, ref remotePoint);
                
                        // 截取有效数据
                        byte[] validBytes = new byte[length];
                        Array.Copy(recvBuffer, validBytes, length);

                        ParsePacket(validBytes);
                    }
                    catch (SocketException sockEx)
                    {
                        // 某些 socket 错误可能不需要退出，比如超时
                        // 但如果是 socket 被关闭了，才需要 break
                        UnityEngine.Debug.Log($"Socket 异常: {sockEx.Message}");
                        // 不要 break，除非你确定连接断开了
                    }
                    catch (Exception e)
                    {
                        // 【重要修复】绝对不要在这里 break！
                        // 打印错误日志，然后允许循环继续，接收下一个包
                        UnityEngine.Debug.LogError($"接收线程发生错误，已忽略: {e.Message}\n{e.StackTrace}");
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
                    case PacketType.PositionAndStatus:
                        UserPositionAndStatusPacket UserPositionAndStatusPacket = new UserPositionAndStatusPacket(reader);
                        UserPositionAndStatusPacketProcess(UserPositionAndStatusPacket);
                        break;
                    case PacketType.ScenesItem:
                        ScenesItemDataPacket scenesItemDataPacket = new ScenesItemDataPacket(reader);
                        ScenesItemDataPacketProcess(scenesItemDataPacket);
                        break;
                    default:
                        break;
                }
            }
        
        }
        
        
        
        
        
        /// <summary>
        /// 处理场景中玩家对象的位置信息更新
        /// </summary>
        /// <param name="remotePoint"></param>
        /// <param name="validBytes"></param>
        public void UserPositionAndStatusPacketProcess(UserPositionAndStatusPacket validBytes)
        {
            // 【新增】放入队列等待 Unity 主线程处理
            _packetQueue.Enqueue(new PacketInfo() { 
                Ip = validBytes.Ip, 
                PositionAndStatusPacket = validBytes
            });
        }

        public void ScenesItemDataPacketProcess(ScenesItemDataPacket validBytes)
        {
            _ScenesIteamDataQueue.Enqueue(validBytes);
        }
        
        
        /// <summary>
        /// 供 Unity 的 Update 调用实时向服务器发送信息
        /// </summary>
        public void Update()
        {
            //处理聊天队列
            while (_msgQueue.TryDequeue(out string msg))
            {
                takeMessage?.Invoke(msg);
            }

            //处理玩家数据包队列
            while (_packetQueue.TryDequeue(out PacketInfo info))
            {
                // 触发事件，将 IP 和 包数据传给 NetworkPlayerManager
                takePlayerPacket?.Invoke(info.Ip, info.PositionAndStatusPacket);
            }

            while (_ScenesIteamDataQueue.TryDequeue(out ScenesItemDataPacket IteamPositionData))
            {
                takeSceneItemPacket.Invoke(IteamPositionData);
            }
        }

        
        
        
        public void Close()
        {
            socket.Close();
        }
    }
