using System;
using Unity.VisualScripting;
using UnityEngine;

public class ClientRoot : SingelBase<ClientRoot>
{
    public StartGamePanel StartGamePanel;
    public NetworkManager NetworkManager;


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
    }

    public void joinGame(String PlayerName)
    {
        GameObject networkManager = Instantiate(Resources.Load<GameObject>("Prefabs/NetWrokManager"));
        NetworkManager = networkManager.GetComponent<NetworkManager>();
        NetworkManager.transform.SetParent(GetParent());
        NetworkManager.GameStart(PlayerName);
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
