using System;
using System.Collections.Generic;
using System.Net;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class ServerAllPlayerManager : MonoBehaviour, IPlayerManager
{
   public Dictionary<string, PlayerInstance> AllPlayerInstance { get; } = new Dictionary<string, PlayerInstance>();
   public Dictionary<string, UserPositionAndStatusPacket> AllPlayerInstancesUserPositionPackets { get; } = new Dictionary<string, UserPositionAndStatusPacket>();
   
   /// <summary>
   /// 供 Unity 的 Update 调用，持续刷新所有玩家的同步数据。
   /// </summary>
   private void Update()
   {
      UpdatePlayerData();
   }
   
   /// <summary>
   /// 根据客户端加入信息创建服务器侧玩家实例。
   /// </summary>
   public void CreatePlayerInstance(string clientKey, UserJoinPacket userJoinPacket)
   {
      GameObject playerInstance = Instantiate(Resources.Load("Prefabs/Player_instance")) as GameObject;
      
      PlayerInstance newPlayerInstance = playerInstance.GetComponent<PlayerInstance>();
      newPlayerInstance.PlayerName = userJoinPacket.name;
      newPlayerInstance.PlayerIp = clientKey;
      AllPlayerInstance.Add(clientKey, newPlayerInstance);
      
      UserPositionAndStatusPacket userPositionPacket = new UserPositionAndStatusPacket(); 
      
      userPositionPacket.Name = newPlayerInstance.PlayerName;
      userPositionPacket.R_X = newPlayerInstance.transform.rotation.eulerAngles.x;
      userPositionPacket.R_Y = newPlayerInstance.transform.rotation.eulerAngles.y;
      userPositionPacket.R_Z = newPlayerInstance.transform.rotation.eulerAngles.z;
      userPositionPacket.X = newPlayerInstance.transform.position.x;
      userPositionPacket.Y = newPlayerInstance.transform.position.y;
      userPositionPacket.Z = newPlayerInstance.transform.position.z;
      userPositionPacket.Ip = clientKey;
      
      AllPlayerInstancesUserPositionPackets.Add(clientKey, userPositionPacket);
   }

   /// <summary>
   /// 移除指定客户端对应的玩家实例与缓存数据。
   /// </summary>
   public void RemovePlayer(string clientKey)
   {
      if (AllPlayerInstance.TryGetValue(clientKey, out var userStatusPacket))
      {
         AllPlayerInstance.Remove(clientKey);
         AllPlayerInstancesUserPositionPackets.Remove(clientKey);
      }
   }

   /// <summary>
   /// 处理指定玩家的移动输入。
   /// </summary>
   public void HandlePlayerMove(string clientKey, UserMovePacket movePacket)
   {
      if (AllPlayerInstance.ContainsKey(clientKey))
      {
         PlayerInstance player = AllPlayerInstance[clientKey];
         player.ApplyMoveInput(movePacket);
      }
   }

   /// <summary>
   /// 触发指定玩家的攻击行为。
   /// </summary>
   public void TriggerPlayerAtacck(string clientKey, EndPoint remoteClient, UserAttackPacket userAttackPacket)
   {
      if (AllPlayerInstance.ContainsKey(clientKey))
      {
         PlayerInstance player = AllPlayerInstance[clientKey];
         player.ApplyAttackInput(userAttackPacket);
      }
   }
   
   /// <summary>
   /// 更新所有玩家当前的位置、朝向与攻击方向缓存。
   /// </summary>
   public void UpdatePlayerData()
   {
      foreach (var playerInstanceKey in AllPlayerInstance)
      {
         PlayerInstance playerInstance = AllPlayerInstance[playerInstanceKey.Key];
         UserPositionAndStatusPacket userPositionPacket = AllPlayerInstancesUserPositionPackets[playerInstanceKey.Key];
         
         userPositionPacket.Name = playerInstance.PlayerName;
         userPositionPacket.R_X = playerInstance.transform.rotation.eulerAngles.x;
         userPositionPacket.R_Y = playerInstance.transform.rotation.eulerAngles.y;
         userPositionPacket.R_Z = playerInstance.transform.rotation.eulerAngles.z;
         userPositionPacket.X = playerInstance.transform.position.x;
         userPositionPacket.Y = playerInstance.transform.position.y;
         userPositionPacket.Z = playerInstance.transform.position.z;
         
         userPositionPacket.Attack_X = playerInstance.FaceDirection.forward.x;
         userPositionPacket.Attack_Y = playerInstance.FaceDirection.forward.y;
         userPositionPacket.Attack_Z = playerInstance.FaceDirection.forward.z;
      }
   }
}
