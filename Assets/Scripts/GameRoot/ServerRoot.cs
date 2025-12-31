
using System;
using Unity.VisualScripting;
using UnityEngine;

public class ServerRoot : MonoBehaviour
{
    private void Start()
    {
        transform.AddComponent<Server>();
    }
}