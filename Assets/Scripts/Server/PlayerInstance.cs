
using System;
using TMPro;
using UnityEngine;

public class PlayerInstance: MonoBehaviour
{
    public string PlayerName;
    public Rigidbody rigidbody;
    public Collider collider;

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
    }

    
    //接受控制指令
    public void Control(string ControlType)
    {
        
    }
    
    
    
}