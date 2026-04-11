using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
    public class PlayerStats : EntityStats
    {
        public int baseRangedAttackDamage = 5;
    }
}
