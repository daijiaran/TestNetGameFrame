using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Conmon
{
    public class GameRoot : SingelBase<GameRoot>
    {
        public bool isServer; 
        private void Awake()
        {
            Init();
            // GameInit();
        }

        //判断当前是服务端还是客户端
        public void GameInit()
        {
            if (isServer) transform.AddComponent<ServerRoot>();            //如果是服务端
            else transform.AddComponent<ClientRoot>();                     //如果是客户端
        }

        public void StartServer()
        {
            transform.AddComponent<ServerRoot>();            //如果是服务端
        }

        public void StartClient()
        {
            transform.AddComponent<ClientRoot>();             //如果是客户端
        }
    }
}
