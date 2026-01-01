using System;
using System.Collections.Generic;
using System.Net;
using Shared.DJRNetLib;
using UnityEngine;

public class ServerAllPlayerManager : MonoBehaviour
{
   //IP地址对应服务器中模拟的玩家当前玩家实例
   public Dictionary<string, PlayerInstance> AllPlayerInstance = new Dictionary<string, PlayerInstance>();
   public Dictionary<string, UserPositionPacket> AllPlayerInstancesUserPositionPackets = new Dictionary<string, UserPositionPacket>();
   public String PlayerPrefabPath;
   
   
   
   
   private void Awake()
   {
      Server.Instance.serviceUpdate.NewPlayerJoinEvent += CreatePlayerInstance;
   }

   private void Update()
   {
      UpdatePlayerData();
   }
   
   
   //创建玩家实体
   public void CreatePlayerInstance(string clientKey ,EndPoint remoteClient,  UserJoinPacket userJoinPacket)
   {
      GameObject playerInstance = Resources.Load<GameObject>(PlayerPrefabPath);
      PlayerInstance newPlayerInstance = playerInstance.GetComponent<PlayerInstance>();
      newPlayerInstance.PlayerName = userJoinPacket.name;
      AllPlayerInstance.Add(clientKey,newPlayerInstance);
      
      
      
      UserPositionPacket userPositionPacket = new UserPositionPacket(); 
      
      userPositionPacket.Name = newPlayerInstance.PlayerName;
      userPositionPacket.R_X = newPlayerInstance.transform.rotation.eulerAngles.x;
      userPositionPacket.R_Y = newPlayerInstance.transform.rotation.eulerAngles.y;
      userPositionPacket.R_Z = newPlayerInstance.transform.rotation.eulerAngles.z;
      userPositionPacket.X = newPlayerInstance.transform.position.x;
      userPositionPacket.Y = newPlayerInstance.transform.position.y;
      userPositionPacket.Z = newPlayerInstance.transform.position.z;
      
      AllPlayerInstancesUserPositionPackets.Add(clientKey,userPositionPacket);
   }

   
   /// <summary>
   /// 持续获取所有玩家的位置信息同步到字典里面
   /// </summary>
   public void UpdatePlayerData()
   {
      foreach (var playerInstanceKey in AllPlayerInstance)
      {
         PlayerInstance playerInstance = AllPlayerInstance[playerInstanceKey.Key];
         UserPositionPacket userPositionPacket = AllPlayerInstancesUserPositionPackets[playerInstanceKey.Key];
         
         userPositionPacket.Name = playerInstance.PlayerName;
         userPositionPacket.R_X = playerInstance.transform.rotation.eulerAngles.x;
         userPositionPacket.R_Y = playerInstance.transform.rotation.eulerAngles.y;
         userPositionPacket.R_Z = playerInstance.transform.rotation.eulerAngles.z;
         userPositionPacket.X = playerInstance.transform.position.x;
         userPositionPacket.Y = playerInstance.transform.position.y;
         userPositionPacket.Z = playerInstance.transform.position.z;
      }
   }
   
   
   
   //为每个玩家实体注入客户端指令
   public void InjectionInstruction()
   {
      
   }
}
