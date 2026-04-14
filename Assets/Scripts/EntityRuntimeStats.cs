using Stats;
using UnityEngine;

[System.Serializable]
public class EntityRuntimeStats
{
    public bool isCustomized;
    
    public int Health;
    public int MaxHealth;
    public int MoveRange;
    public float MoveSpeed;
    public int AttackDamage;

    public EntityRuntimeStats() { }

    public void Initialize(EntityStats so)
    {
        if (so == null) return;
        MaxHealth = so.maxHealth;
        Health = MaxHealth;
        MoveRange = so.moveRange;
        MoveSpeed = so.moveSpeed;
        AttackDamage = so.baseAttackDamage;
    }
    
    public bool IsDead => Health <= 0;
    
    public void ApplyDamage(int amount)
    {
        Health = Mathf.Max(0, Health - amount);
    }
    
    public void ApplyHeal(int amount)
    {
        Health = Mathf.Min(MaxHealth, Health + amount);
    }
}