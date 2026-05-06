using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using UnityEngine;

namespace MyNetGame.ServerScenesOBJ
{
    public class ServerScenesObjectBase : MonoBehaviour
    {
        public string PrefabsName;
        public int CurrentInstanceID;
        public bool IsDestroy;

        protected bool IsInitialized { get; private set; }
        
        protected ISceneObjectManager sceneObjectManager;
        protected INetworkService networkService;

        /// <summary>
        /// 初始化场景对象所依赖的场景管理器与网络服务。
        /// </summary>
        public virtual void Initialize(ISceneObjectManager sceneManager, INetworkService network)
        {
            this.sceneObjectManager = sceneManager;
            this.networkService = network;
        }

        /// <summary>
        /// 完成场景对象注册，并将其加入服务器同步管理。
        /// </summary>
        public virtual void InitOBJ()
        {
            IsInitialized = true;
            if (sceneObjectManager != null)
            {
                sceneObjectManager.AddAllItem(transform, PrefabsName);
            }
        }

        /// <summary>
        /// 销毁当前场景对象实例。
        /// </summary>
        public void Die()
        {
            Debug.Log("执行销毁:" + transform.name);
            Destroy(gameObject);
        }

        /// <summary>
        /// 在对象禁用时移除场景数据并向客户端广播销毁状态。
        /// </summary>
        private void OnDisable()
        {
            if (!IsInitialized || sceneObjectManager == null || networkService == null)
            {
                return;
            }

            if (sceneObjectManager.AllItemsTransData.TryGetValue(CurrentInstanceID, out var scenesItemPacket))
            {
                scenesItemPacket.isDestroy = true;
                sceneObjectManager.RemoveScenesItem(CurrentInstanceID);
                networkService.SendToAllPlayerDestoryOBJ(PacketType.ScenesItem, scenesItemPacket);
            }
        }
    }
}
