
using UnityEngine;

public class ScenesItemBase:MonoBehaviour
{
    public string PrefabsName;
    public int CurrentInstanceID;
    public bool IsServer;
    public bool IsDestroy;
    
    public virtual void InitOBJ()
    {
        
    }

    public void Die()
    {
        Debug.Log("执行销毁"+transform.name);
        Destroy(gameObject);
    }
}
