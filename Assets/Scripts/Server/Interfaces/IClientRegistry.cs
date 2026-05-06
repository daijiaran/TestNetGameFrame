using System.Collections.Generic;
using System.Net;

public interface IClientRegistry
{
    /// <summary>
    /// 添加或更新客户端对应的网络终结点。
    /// </summary>
    void Upsert(string clientKey, EndPoint remoteClient);

    /// <summary>
    /// 尝试根据客户端标识获取对应的网络终结点。
    /// </summary>
    bool TryGet(string clientKey, out EndPoint endPoint);

    /// <summary>
    /// 获取全部已注册客户端的网络终结点数据。
    /// </summary>
    IReadOnlyDictionary<string, EndPoint> GetAll();

    /// <summary>
    /// 移除指定客户端的注册信息。
    /// </summary>
    void Remove(string clientKey);
}
