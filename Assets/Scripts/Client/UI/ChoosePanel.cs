using Conmon;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePanel : MonoBehaviour
{
    public Button ServiceButton;
    public Button ClientButton;
    
    
    void Start()
    {
        ServiceButton.onClick.AddListener(Service);
        ClientButton.onClick.AddListener(Client);
    }


    public void Service()
    {
        GameRoot.Instance.StartServer();
    }

    public void Client()
    {
        GameRoot.Instance.StartClient();
    }
}
