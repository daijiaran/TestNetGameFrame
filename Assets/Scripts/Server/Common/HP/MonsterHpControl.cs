using System;

public class MonsterHpControl:HPBase
{
    /// <summary>
    /// 在对象启动时初始化怪物的最大生命值与当前生命值。
    /// </summary>
    private void Start()
    {
        MaxHealth = 10;
        CurrentHealth = MaxHealth;
    }
}
