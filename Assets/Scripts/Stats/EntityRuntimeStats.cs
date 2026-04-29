using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public class EntityRuntimeStats
    {
        public bool isCustomized;

        public int Freeze;
        public int Health;
        public int MaxHealth;
        public int Energy;
        public int MaxEnergy;
        public int MoveRange;
        public float MoveSpeed;
        public int AttackDamage;
        public int PreferredAttackRange;
        public int MinimumRange;

        public EntityRuntimeStats()
        {
        }

        public void Initialize(EntityStats so)
        {
            if (so == null) return;
            MaxHealth = so.maxHealth;
            Health = MaxHealth;

            MaxEnergy = so.maxEnergy;
            Energy = MaxEnergy;

            MoveRange = so.moveRange;
            MoveSpeed = so.moveSpeed;
            AttackDamage = so.baseAttackDamage;

            PreferredAttackRange = so.preferredAttackRange;
            MinimumRange = so.minimumRange;
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
}