

using System;
using System.Collections.Generic;
using Shared.DJRNetLib.Packet;
using UnityEngine;

public class NetworkScensItemManager:MonoBehaviour
{
    public Dictionary<int,Transform> AllItemInstance = new Dictionary<int, Transform>();
    NetConect  netConect;
    private void Start()
    {
        netConect = ClientRoot.Instance.netConect;
        //为同步场景中其他对象事件注册方法
        netConect.takeSceneItemPacket+=UpdateScenesItemTransform;
    }


    public void UpdateScenesItemTransform(ScenesItemDataPacket sceneItemData)
    {
        int key = sceneItemData.ItemIndex;
        string ItemName  = sceneItemData.ItemName;
        if (AllItemInstance.ContainsKey(key))
        {
            
            Transform iteTransform = AllItemInstance[key];
            if (sceneItemData.isDestroy)
            {
                ClientRoot.Instance.networkScensItemManager.AllItemInstance.Remove(sceneItemData.ItemIndex);
                Debug.Log("销毁物体"+iteTransform.name);
                iteTransform.GetComponent<ScenesItemBase>().Die();
            }
            
            float X, Y, Z; 
            float R_x, R_y, R_z;
            X =  sceneItemData.X;
            Y =  sceneItemData.Y;
            Z =  sceneItemData.Z;
            R_x =  sceneItemData.R_x;
            R_y =  sceneItemData.R_y;
            R_z =  sceneItemData.R_z;
            
            Vector3 pos = new Vector3(X,Y,Z);
            Vector3 rot = new Vector3(R_x,R_y,R_z);
            
            iteTransform.transform.position = pos;
            iteTransform.transform.rotation = Quaternion.Euler(rot);
        }
        else
        {
            CreatScenesItem(key, sceneItemData);
        }
        
        
    }
    
    
    public void CreatScenesItem(int key , ScenesItemDataPacket sceneItemData)
    {
        float X, Y, Z; 
        float R_x, R_y, R_z;
        X =  sceneItemData.X;
        Y =  sceneItemData.Y;
        Z =  sceneItemData.Z;
        R_x =  sceneItemData.R_x;
        R_y =  sceneItemData.R_y;
        R_z =  sceneItemData.R_z;
        Vector3 pos = new Vector3(X,Y,Z);
        Vector3 rot = new Vector3(R_x,R_y,R_z);
        
        GameObject prefab = Resources.Load<GameObject>("Prefabs/"+sceneItemData.ItemName);
        if (prefab != null)
        {
            GameObject ItemInstance = Instantiate(prefab, pos, Quaternion.Euler(rot));
            ItemInstance.GetComponent<ScenesItemBase>().InitOBJ();
            AllItemInstance.Add(key,ItemInstance.transform);
        }
        else
        {
            Debug.Log("没有找到指定预制体，无法生成对象并同步");
        }
        
        
    }
}