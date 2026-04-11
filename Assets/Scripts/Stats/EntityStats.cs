using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "EntityStats", menuName = "Scriptable Objects/EntityStats")]
    public class EntityStats : ScriptableObject
    {
        public int maxHealth = 10;
        public int moveRange = 1;
        public float moveSpeed = 3f;
        public int baseAttackDamage = 5;
    }
}
