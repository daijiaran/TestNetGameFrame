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

    public Action GameOverEvent;
    

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


        GameOverEvent += GameOver;
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


    public void GameOver()
    {
        GameObject GamoverPanel = Instantiate(Resources.Load<GameObject>("Prefabs/GameOverPanel"));
        GamoverPanel.transform.SetParent(GetParent());
    
        RectTransform rectTransform = GamoverPanel.GetComponent<RectTransform>();

        // 1. 设置本地位移、旋转和缩放为初始值
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        // 2. 将所有边距（Left, Right, Top, Bottom）归零
        // offsetMin 对应 Left (x) 和 Bottom (y)
        // offsetMax 对应 Right (-x) 和 Top (-y)
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }


}
