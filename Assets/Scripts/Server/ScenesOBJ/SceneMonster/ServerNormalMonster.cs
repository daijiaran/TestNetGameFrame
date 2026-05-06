using UnityEngine;

namespace MyNetGame.ServerScenesOBJ
{
    public class ServerNormalMonster : ServerMonsterBase
    {
        private int updateInterval = 10;
        private int frameCount;

        /// <summary>
        /// 供 Unity 固定帧调用，周期性刷新目标并驱动怪物追踪逻辑。
        /// </summary>
        private void FixedUpdate()
        {
            if (playerManager == null)
            {
                return;
            }

            frameCount++;
            if (frameCount >= updateInterval)
            {
                frameCount = 0;
                GetPlayerList();
            }

            if (Playerlist.Count > 0)
            {
                FindPlayerMinDistance();
                MoveToTarget();
            }
        }
    }
}
