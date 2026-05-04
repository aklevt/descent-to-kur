using System.Collections;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Враг дальнего боя. Держится на дистанции и стреляет снарядами
    /// </summary>
    public class RangedEnemy : EnemyBase
    {
        [Header("Ranged Enemy Settings")]
        [Tooltip("Смещение точки спауна снаряда по X от центра")]
        [SerializeField] private float spawnOffsetX = 0.34f;
        
        [Tooltip("Смещение точки спауна снаряда по Y от центра")]
        [SerializeField] private float spawnOffsetY = 0.85f;
        
        protected override IEnumerator ExecuteAction()
        {
            yield return TryUseAbility(0);
        }

        /// <summary>
        /// Точка спауна снаряда снаряда с учетом flip
        /// </summary>
        public Vector3 GetProjectileSpawnPosition()
        {
            var offsetX = (spriteRenderer != null && spriteRenderer.flipX) 
                ? -spawnOffsetX 
                : spawnOffsetX;
            
            return transform.position + new Vector3(offsetX, spawnOffsetY, 0);
        }
    }
}