using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "EntityStats", menuName = "Scriptable Objects/EntityStats")]
    public class EntityStats : ScriptableObject
    {
        public int maxHealth = 20;
        public int maxEnergy = 3;
        public int moveRange = 1;
        public int maxStepsPerRound = 3; // Макс. количество ходов для игрока за раунд 
        public float moveSpeed = 3f;
		
		/// <summary>
        /// Базовая сила атаки ближнего боя 
        /// Итоговый урон способности будет рассчитываться как: 
        /// (baseAttackDamage * abilityMultiplier) + abilityLinearModifier, возможно без abilityMultiplier
        /// </summary>
        public int baseAttackDamage = 5;
		
		/// <summary>
        /// Базовая сила дистанционной атаки, сейчас не нужна
        /// Возможно, будет использоваться для способностей дальней атаки
        /// </summary>
        public int rangedAttackDamage = 5;
		
        [Tooltip("На какой дистанции враг предпочитает атаковать")]
        public int preferredAttackRange = 1;
        
        [Tooltip("Минимальная дистанция до игрока. 0 = не убегать")]
        public int minimumRange = 0;
    }
}
