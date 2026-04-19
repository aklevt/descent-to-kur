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
        public int baseAttackDamage = 5;
        public int baseRangedAttackDamage = 5;
    }
}
