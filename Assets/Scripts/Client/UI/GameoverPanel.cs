using System;
using Shared.DJRNetLib.Common;
using UnityEngine;
using UnityEngine.UI;

public class GameoverPanel : MonoBehaviour
{
   public Button AgainButton;

   private void Start()
   {
      AgainButton.onClick.AddListener(AgainButtonClick);
   }

   public void AgainButtonClick()
   {
      PlayerGameData gameData = ClientRoot.Instance.networkPlayerManager.playerGameData;
      ClientRoot.Instance.networkPlayerManager.GameStart(gameData.name);
      Destroy(gameObject);
   }
}
