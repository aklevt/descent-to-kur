using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "EntityStats", menuName = "Scriptable Objects/EntityStats")]
    public class EntityStats : ScriptableObject
    {
        public int MaxHealth;
        public int MaxEnergy;
        //public int moveRange = 1;
        public float AnimationSpeed;
		
		/// <summary>
        /// Базовая сила атаки ближнего боя 
        /// Итоговый урон способности будет рассчитываться как: 
        /// (baseAttackDamage * abilityMultiplier) + abilityLinearModifier
        /// </summary>
        //public int baseAttackDamage = 5;
		
		/// <summary>
        /// Базовая сила дистанционной атаки
        /// Возможно, будет использоваться для способностей дальней атаки
        /// </summary>
        //public int baseRangedAttackDamage = 5;
    }
}
