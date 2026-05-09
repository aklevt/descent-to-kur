using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public class EntityRuntimeStats
    {
        [Header("⚙️ Configuration")]
        [Tooltip("Если true, статы были изменены вручную в инспекторе")]
        public bool isCustomized;

        [Header("Common")]
        public int Health;
        public int MaxHealth;
        public int MoveRange;
        public int AttackDamage;
        public int Freeze;

        [Header("Player Only")]
        public int Energy;
        public int MaxEnergy;
        public int RemainingSteps;
        public int MaxStepsPerRound;

        [Header("Enemies only")]
        [Tooltip("Предпочитаемая дистанция атаки")]
        public int PreferredAttackRange;
        [Tooltip("Минимальная дистанция до игрока (0 = не отступать)")]
        public int MinimumRange;

        public EntityRuntimeStats()
        {
        }

        public void Initialize(EntityStatsBase baseStats)
        {
            if (baseStats == null)
            {
                Debug.LogWarning("[EntityRuntimeStats]");
                return;
            }

            // Базовые статы
            MaxHealth = baseStats.maxHealth;
            Health = MaxHealth;
            MoveRange = baseStats.moveRange;
            AttackDamage = baseStats.baseAttackDamage;
            Freeze = 0;

            // Статы игрока
            if (baseStats is PlayerStatsData playerData)
            {
                MaxEnergy = playerData.maxEnergy;
                Energy = MaxEnergy;
                MaxStepsPerRound = playerData.maxStepsPerRound;
                RemainingSteps = MaxStepsPerRound;
            }

            // Статы врагов
            if (baseStats is EnemyStatsData enemyData)
            {
                PreferredAttackRange = enemyData.preferredAttackRange;
                MinimumRange = enemyData.minimumRange;
            }
        }

        public bool IsDead => Health <= 0;

        public bool HasEnergyForAction(int cost) => Energy >= cost;
        public void SpendEnergy(int amount) => Energy = Mathf.Max(0, Energy - amount);
        public void RestoreEnergy(int amount) => Energy = Mathf.Min(MaxEnergy, Energy + amount);
        
        public void ResetSteps(int maxSteps) => RemainingSteps = maxSteps;
        public bool CanMove(int distance) => RemainingSteps >= distance;
        public void SpendSteps(int distance) => RemainingSteps = Mathf.Max(0, RemainingSteps - distance);

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