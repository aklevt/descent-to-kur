using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "EntityStats", menuName = "Scriptable Objects/EntityStats")]
    public class EntityStats : ScriptableObject
    {
        public int maxHealth = 20;
        public int moveRange = 1;
        public float moveSpeed = 3f;
        // Не факт, что у всех сущностей будет атакующая способность (по крайней мере дальняя)
        public int baseAttackDamage = 5;
        public int baseRangedAttackDamage = 5;
    }
}
