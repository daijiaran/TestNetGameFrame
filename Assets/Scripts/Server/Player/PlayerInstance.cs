using System;
using MyNetGame.ServerScenesOBJ;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    private IPlayerManager playerManager;
    private ISceneObjectManager sceneObjectManager;
    private INetworkService networkService;
    
    public string PlayerName;
    public string PlayerIp;
    public Rigidbody rigidbody;
    public Collider collider;
    public float moveSpeed = 5f;
    [Header("枪口的面向方向")]
    public Transform FaceDirection;
    public Transform BullettPos;
    public PlayerHpControl PlayerHpControl;

    /// <summary>
    /// 初始化玩家实例依赖的管理器与网络服务。
    /// </summary>
    public void Initialize(IPlayerManager manager, ISceneObjectManager sceneManager, INetworkService network)
    {
        this.playerManager = manager;
        this.sceneObjectManager = sceneManager;
        this.networkService = network;
    }
    
    /// <summary>
    /// 在对象启动时缓存组件并初始化生命值控制器。
    /// </summary>
    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
        PlayerHpControl = transform.AddComponent<PlayerHpControl>();
        PlayerHpControl.IsDead += Die;
    }

    /// <summary>
    /// 应用客户端发送的移动输入并更新玩家朝向。
    /// </summary>
    public void ApplyMoveInput(UserMovePacket movePacket)
    {
        Vector3 forward = new Vector3(movePacket.D_x, 0, movePacket.D_z);
        if (forward.sqrMagnitude < 0.0001f)
            return;

        forward.Normalize();
        
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 moveDir = forward * movePacket.V + right * movePacket.H;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        if (rigidbody != null)
        {
            rigidbody.linearVelocity = new Vector3(
                moveDir.x * moveSpeed,
                rigidbody.linearVelocity.y,
                moveDir.z * moveSpeed
            );

            if (moveDir.sqrMagnitude > 0.001f)
            {
                transform.forward = moveDir;
            }
        }
        
        FaceDirection.forward = new Vector3(movePacket.Attack_x, 0, movePacket.Attack_z);
    }

    /// <summary>
    /// 应用客户端发送的攻击输入并生成对应子弹对象。
    /// </summary>
    public void ApplyAttackInput(UserAttackPacket attackPacket)
    {
        if (attackPacket.BulltType == 1)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + attackPacket.Prefabsname);
            if (prefab == null) 
            {
                Debug.LogError("找不到子弹预制体！请检查路径：Prefabs/" + attackPacket.Prefabsname);
                return;
            }
             
            GameObject bulletObj = Instantiate(prefab, BullettPos.position, FaceDirection.rotation);
            
            ServerBulltControl bulletScript = bulletObj.GetComponent<ServerBulltControl>();
            bulletScript.PrefabsName = attackPacket.Prefabsname;
            
            if (sceneObjectManager != null && networkService != null)
            {
                bulletScript.Initialize(sceneObjectManager, networkService);
            }
            
            bulletScript.InitOBJ();
            
            Rigidbody bulletRb = bulletObj.GetComponent<Rigidbody>();

            if (bulletRb != null && bulletScript != null)
            {
                float speed = bulletScript.BulltVelocity > 0 ? bulletScript.BulltVelocity : 20f;
                bulletRb.linearVelocity = FaceDirection.forward * speed; 
            }

            Collider playerCollider = GetComponent<Collider>();
            Collider bulletCollider = bulletObj.GetComponent<Collider>();
        
            if (playerCollider != null && bulletCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, bulletCollider);
            }
        }
    }

    /// <summary>
    /// 处理玩家死亡并销毁当前玩家对象。
    /// </summary>
    public void Die()
    {
        Destroy(this.gameObject);
    }
    
    /// <summary>
    /// 在玩家对象禁用时清理数据并向其他客户端广播死亡状态。
    /// </summary>
    private void OnDisable()
    {
        if (playerManager == null || networkService == null) return;
        
        if (playerManager.AllPlayerInstancesUserPositionPackets.TryGetValue(PlayerIp, out var userStatusPacket))
        {
            playerManager.RemovePlayer(PlayerIp);
            userStatusPacket.isDead = true;
            networkService.SendToAllPlayerDestoryOBJ(PacketType.PositionAndStatus, userStatusPacket);
        }
    }
}
