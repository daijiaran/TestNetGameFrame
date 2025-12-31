using UnityEngine;

public class Server : MonoBehaviour
{
    public ServiceUpdate serviceUpdate;
    void Start()
    {
        Debug.Log("服务器开始运行！！！");        
        serviceUpdate = new ServiceUpdate();
    }

    void Update()
    {
        serviceUpdate.Update();
    }
}
