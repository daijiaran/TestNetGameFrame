

using System;
using UnityEngine;

public class HPBase:MonoBehaviour
{
    public float MaxHealth = 100;
    public float CurrentHealth = 100;
    
    public Action IsDead;
    
    /// <summary>
    /// 让当前对象受到伤害，并在生命值归零时触发死亡事件。
    /// </summary>
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            IsDead?.Invoke();
        }
    }
}
