using System;
using Shared.DJRNetLib;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetTalk : MonoBehaviour
{
   public GameObject MessagePrefab;
   public NetConect netConect =  new NetConect();
   public TextMeshProUGUI textMeshProUGUI;

   private void Start()
   {
      netConect.takeMessage += MakeMasege;
      //开启监听的线程
      netConect.ReceiveInformation();
   }

   public void Update()
   {  
      netConect.Update();
      SendMessage();
   }

   public void SendMessage()
   {
      if (Keyboard.current.enterKey.wasPressedThisFrame)
      { 
         MakeMasege("我："+textMeshProUGUI.text);
         netConect.SendMessage(textMeshProUGUI.text);
         textMeshProUGUI.text = null;
      }
   }
   
   
   public void MakeMasege( String str)
   {
      GameObject Message =  Instantiate(MessagePrefab);
      Message.GetComponent<Text>().text = str;
      Message.transform.SetParent(transform);
   }
   
   
   
   
}
