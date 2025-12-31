using UnityEngine;


/// <summary>
/// 单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingelBase<T>: MonoBehaviour where T : class
{
   public static T Instance;

   public virtual void Init()
   {
      if (Instance != null)
      {
         if (Instance != (this as T)) 
         {
            Debug.LogWarning($"场景中存在重复的单例: {typeof(T).Name}，正在销毁重复对象。");
            Destroy(gameObject);
            return; 
         }
      }
      Instance = this as T;
   }
}


