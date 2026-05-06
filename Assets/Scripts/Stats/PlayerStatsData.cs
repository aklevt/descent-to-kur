using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats/Player Stats")]
    public class PlayerStatsData : EntityStatsBase
    {
        [Header("Player stats")]
        [Tooltip("Максимальная энергия")]
        public int maxEnergy = 10;
        
        [Tooltip("Максимум шагов за раунд")]
        public int maxStepsPerRound = 3;
    }
}