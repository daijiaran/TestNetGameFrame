using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ClientRoot : SingelBase<ClientRoot>
{
    public StartGamePanel StartGamePanel;
    [FormerlySerializedAs("NetworkPlayerManager")] public NetworkPlayerManager networkPlayerManager;
    [FormerlySerializedAs("NetworkScensItemManager")] public NetworkScensItemManager networkScensItemManager;
    
    
    
    [Header("网络层服务")] public NetConect  netConect;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        
        GameObject starpanel = new GameObject();
        starpanel = Instantiate(Resources.Load<GameObject>("Prefabs/GameStart"));
        starpanel.transform.SetParent(GetParent());
        starpanel.transform.localPosition = Vector3.zero;
        StartGamePanel = starpanel.GetComponent<StartGamePanel>();
        StartGamePanel.GameStarteEvent += joinGame;
        
        netConect = new NetConect();
        //开启消息接受线程
        netConect.ReceiveInformation();

    }

    public void joinGame(String PlayerName)
    {
        GameObject networkPlayerManager_OBJ = Instantiate(Resources.Load<GameObject>("Prefabs/NetworkPlayerManager"));
        GameObject networkScensItemManager_OBJ = Instantiate(Resources.Load<GameObject>("Prefabs/NetworkScensItemManager"));

        networkPlayerManager = networkPlayerManager_OBJ.GetComponent<NetworkPlayerManager>();
        networkScensItemManager =  networkScensItemManager_OBJ.GetComponent<NetworkScensItemManager>();
        
        
        
        //游戏开始由玩家同步组件开启
        networkPlayerManager.GameStart(PlayerName);
    }


    public Transform GetParent()
    {
        foreach (Transform child in transform)
        {
            if (child.transform.name == "Canvas")
            {
                return child.transform;
            }
        }
        return null;
    }


}
