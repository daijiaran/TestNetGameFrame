using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGamePanel : MonoBehaviour
{
    public Button StartButton;
    public TextMeshProUGUI NameText;
    public Action<String> GameStarteEvent;
    
    
    public void Start()
    {
        StartButton.onClick.AddListener(StartGameVoid);
    }


    public void StartGameVoid()
    {
        if (NameText != null)
        {
            if (NameText.text != "")
            {
                GameStarteEvent.Invoke(NameText.text);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("用户名字不能为空！！！");
            }
        }
    }
        
}
