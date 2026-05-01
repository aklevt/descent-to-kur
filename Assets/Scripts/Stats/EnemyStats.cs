using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
    public class EnemyStats : EntityStats
    {
        public int moveRange;
    }
}
