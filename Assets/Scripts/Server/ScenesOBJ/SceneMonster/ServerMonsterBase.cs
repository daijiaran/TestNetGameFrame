using System.Collections.Generic;
using UnityEngine;

namespace MyNetGame.ServerScenesOBJ
{
    public class ServerMonsterBase : ServerScenesObjectBase
    {
        public List<global::PlayerInstance> Playerlist = new List<global::PlayerInstance>();
        protected IPlayerManager playerManager;
        public global::PlayerInstance TargetPlayerInstance;
        public float moveSpeed;
        public global::MonsterHpControl hpControl;

        /// <summary>
        /// 初始化怪物对象所需的玩家管理器、场景管理器与网络服务。
        /// </summary>
        public virtual void InitializeMonster(IPlayerManager manager, ISceneObjectManager sceneManager, INetworkService network)
        {
            base.Initialize(sceneManager, network);
            this.playerManager = manager;
        }

        /// <summary>
        /// 初始化怪物对象并挂载生命值控制组件。
        /// </summary>
        public override void InitOBJ()
        {
            base.InitOBJ();
            if (playerManager != null)
            {
                hpControl = gameObject.AddComponent<global::MonsterHpControl>();
                hpControl.IsDead += Die;
            }
        }

        /// <summary>
        /// 刷新当前场景中的玩家目标列表。
        /// </summary>
        public void GetPlayerList()
        {
            Playerlist.Clear();
            if (playerManager == null) return;
            
            foreach (var value in playerManager.AllPlayerInstance.Values)
            {
                Playerlist.Add(value);
            }
        }

        /// <summary>
        /// 在当前玩家列表中寻找距离最近的目标。
        /// </summary>
        public void FindPlayerMinDistance()
        {
            if (Playerlist == null || Playerlist.Count == 0)
            {
                return;
            }

            var targetInstance = Playerlist[0];
            foreach (var player in Playerlist)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < Vector3.Distance(transform.position, targetInstance.transform.position))
                {
                    targetInstance = player;
                }
            }

            TargetPlayerInstance = targetInstance;
        }

        /// <summary>
        /// 让怪物朝当前目标玩家移动并同步朝向。
        /// </summary>
        public void MoveToTarget()
        {
            if (TargetPlayerInstance == null)
            {
                return;
            }

            Vector3 targetPos = TargetPlayerInstance.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
    }
}
