using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Animation Speed")]
        [Range(0.1f, 5f)]
        public float globalAnimationSpeedMultiplier = 1f;
        
        [Header("Movement")]
        [Range(1f, 20f)]
        [Tooltip("Базовая скорость передвижения всех сущностей")]
        public float baseMoveSpeed = 3f;
    
        [Header("Turn Settings")]
        public float enemyTurnDelay = 0.1f;
        public float roundEndDelay = 0.1f;
    }
}