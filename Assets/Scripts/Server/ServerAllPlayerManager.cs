using System;
using System.Collections.Generic;
using System.Net;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class ServerAllPlayerManager : MonoBehaviour
{
   //IP地址对应服务器中模拟的玩家当前玩家实例
   public Dictionary<string, PlayerInstance> AllPlayerInstance = new Dictionary<string, PlayerInstance>();
   public Dictionary<string, UserPositionAndStatusPacket> AllPlayerInstancesUserPositionPackets = new Dictionary<string, UserPositionAndStatusPacket>();
   
   private void Update()
   {
      UpdatePlayerData();
   }
   
   
   //创建玩家实体
   public void CreatePlayerInstance(string clientKey ,  UserJoinPacket userJoinPacket)
   {
      GameObject playerInstance = Instantiate(Resources.Load("Prefabs/Player_instance")) as GameObject;
      
      PlayerInstance newPlayerInstance = playerInstance.GetComponent<PlayerInstance>();
      newPlayerInstance.PlayerName = userJoinPacket.name;
      newPlayerInstance.PlayerIp = clientKey;
      AllPlayerInstance.Add(clientKey,newPlayerInstance);
      
      
      
      UserPositionAndStatusPacket userPositionPacket = new UserPositionAndStatusPacket(); 
      
      userPositionPacket.Name = newPlayerInstance.PlayerName;
      userPositionPacket.R_X = newPlayerInstance.transform.rotation.eulerAngles.x;
      userPositionPacket.R_Y = newPlayerInstance.transform.rotation.eulerAngles.y;
      userPositionPacket.R_Z = newPlayerInstance.transform.rotation.eulerAngles.z;
      userPositionPacket.X = newPlayerInstance.transform.position.x;
      userPositionPacket.Y = newPlayerInstance.transform.position.y;
      userPositionPacket.Z = newPlayerInstance.transform.position.z;
      userPositionPacket.Ip = clientKey;
      
      AllPlayerInstancesUserPositionPackets.Add(clientKey,userPositionPacket);
   }

   
   /// <summary>
   /// 为每个玩家注入移动的指令
   /// </summary>
   /// <param name="clientKey"></param>
   /// <param name="movePacket"></param>
   public void HandlePlayerMove(string clientKey, UserMovePacket movePacket)
   {
      if (AllPlayerInstance.ContainsKey(clientKey))
      {
         PlayerInstance player = AllPlayerInstance[clientKey];
         // 传递输入给玩家实例
         player.ApplyMoveInput(movePacket);
      }
   }

   /// <summary>
   /// 为每个玩家注入攻击指令
   /// </summary>
   /// <param name="clientKey"></param>
   /// <param name="remoteClient"></param>
   /// <param name="userAttack"></param>
   public void TriggerPlayerAtacck(string clientKey ,EndPoint remoteClient,UserAttackPacket userAttackPacket)
   {
      if (AllPlayerInstance.ContainsKey(clientKey))
      {
         PlayerInstance player = AllPlayerInstance[clientKey];
         player.ApplyAttackInput(userAttackPacket);
      }
   }
   
   
   /// <summary>
   /// 持续获取所有玩家的位置信息同步到字典里面
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
