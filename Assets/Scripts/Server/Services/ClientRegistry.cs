using System.Collections.Generic;
using System.Net;

public class ClientRegistry : IClientRegistry
{
    private readonly Dictionary<string, EndPoint> clientEndPoints = new Dictionary<string, EndPoint>();

    /// <summary>
    /// 添加或更新客户端对应的网络终结点。
    /// </summary>
    public void Upsert(string clientKey, EndPoint remoteClient)
    {
        clientEndPoints[clientKey] = remoteClient;
    }

    /// <summary>
    /// 尝试根据客户端标识获取对应的网络终结点。
    /// </summary>
    public bool TryGet(string clientKey, out EndPoint endPoint)
    {
        return clientEndPoints.TryGetValue(clientKey, out endPoint);
    }

    /// <summary>
    /// 获取当前已注册的全部客户端终结点数据。
    /// </summary>
    public IReadOnlyDictionary<string, EndPoint> GetAll()
    {
        return clientEndPoints;
    }

    /// <summary>
    /// 移除指定客户端的注册信息。
    /// </summary>
    public void Remove(string clientKey)
    {
        clientEndPoints.Remove(clientKey);
    }
}
