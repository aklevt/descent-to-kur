using Stats;
using UnityEngine;

[System.Serializable]
public class EntityRuntimeStats
{
    public bool isCustomized;
    
    public int Freeze;
    public int Health;
    public int MaxHealth;
    public int Energy;
    public int MaxEnergy;
    public float AnimationSpeed;

    public EntityRuntimeStats()
    {
    }

    public void Initialize(EntityStats so)
    {
        if (so == null) return;
        MaxHealth = so.MaxHealth;
        Health = MaxHealth;
        MaxEnergy = so.MaxEnergy; 
        Energy = MaxEnergy;
        AnimationSpeed = so.AnimationSpeed;
    }

    public bool IsDead => Health <= 0;
    
    public bool HasEnergyForAction(int cost) => Energy >= cost;
    public void SpendEnergy(int amount) => Energy = Mathf.Max(0, Energy - amount);
    public void RestoreEnergy(int amount) => Energy = Mathf.Min(MaxEnergy, Energy + amount);

    public void ApplyDamage(int amount)
    {
        Health = Mathf.Max(0, Health - amount);
    }

    public void ApplyHeal(int amount)
    {
        Health = Mathf.Min(MaxHealth, Health + amount);
    }
}