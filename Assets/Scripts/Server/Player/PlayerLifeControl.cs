

using System;

public class PlayerLifeControl
{
    public float MaxHealth = 100;
    public float CurrentHealth = 100;
    
    public Action IsDead;
    
    public void TakeDamage(float damage)
    {
        if(CurrentHealth>=damage) CurrentHealth-= damage;
        else IsDead.Invoke();
    }
    
    
}