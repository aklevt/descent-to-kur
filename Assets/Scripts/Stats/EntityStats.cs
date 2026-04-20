using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "EntityStats", menuName = "Scriptable Objects/EntityStats")]
    public class EntityStats : ScriptableObject
    {
        public int maxHealth = 20;
        public int maxEnergy = 3;
        public int moveRange = 1;
        public float moveSpeed = 3f;
		
		/// <summary>
        /// Базовая сила атаки ближнего боя 
        /// Итоговый урон способности будет рассчитываться как: 
        /// (baseAttackDamage * abilityMultiplier) + abilityLinearModifier
        /// </summary>
        public int baseAttackDamage = 5;
		
		/// <summary>
        /// Базовая сила дистанционной атаки
        /// Возможно, будет использоваться для способностей дальней атаки
        /// </summary>
        public int baseRangedAttackDamage = 5;
    }
}
