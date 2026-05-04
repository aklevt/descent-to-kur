using System.Collections;
using System.Collections.Generic;
using Entities;
using FX;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "RangedAttackAbility", menuName = "Abilities/RangedAttack")]
    public class RangedAttackAbilityData : AbilityData
    {
        [Header("Range")]
        public int minRange = 2;
        public int maxRange = 4;

        [Header("Projectile")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 10f;
        
        [Header("Charge Effect")]
        [SerializeField] private GameObject chargeEffectPrefab;
        [SerializeField] private float chargeTime = 0.3f;

        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int origin, BaseEntity actor)
        {
            return GridManager.Instance.GetAttackableCellsInRadius(origin, maxRange, minRange);
        }

        public override List<Vector3Int> GetEffectCells(Vector3Int hoveredCell, BaseEntity actor)
            => new List<Vector3Int> { hoveredCell };
        
        public override Vector3Int? ChooseTarget(BaseEntity actor)
        {
            var playerCell = PlayerMovement.Instance?.CurrentCell;
            if (playerCell == null) return null;

            var available = GetTargetCells(actor);
            return available.Contains(playerCell.Value) ? playerCell : null;
        }
        
        /// <summary>
        /// Требует наличие цели на клетке, так как это не aoe-атака
        /// </summary>
        public override bool IsValidTarget(Vector3Int targetCell, BaseEntity actor)
        {
            var target = GridManager.Instance.GetEntityAt(targetCell);
            return target != null;
        }
        
        public override IEnumerator Execute(BaseEntity actor, Vector3Int targetCell)
        {
            var targetObj = GridManager.Instance.GetEntityAt(targetCell);
            if (targetObj == null) yield break;

            var targetPos = targetObj.transform.position + new Vector3(0, 0.5f, 0);
            var damage = GetCalculatedDamage(actor);
            
            actor.FlipToTarget(targetPos);
            
            yield return new WaitForSeconds(actor.GetScaledTime(0.1f));
            
            var spawnPos = actor.transform.position;
            if (actor is RangedEnemy rangedEnemy)
            {
                spawnPos = rangedEnemy.GetProjectileSpawnPosition();
            }
            else if (actor is PlayerMovement player)
            {
                spawnPos = player.GetProjectileSpawnPosition();
            }

            GameObject chargeEffect = null;
            if (chargeEffectPrefab != null)
            {
                chargeEffect = Instantiate(chargeEffectPrefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(actor.GetScaledTime(chargeTime));
                Destroy(chargeEffect);
            }

            // Запуск снаряда
            if (projectilePrefab != null)
            {
                var projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                var projectile = projectileObj.GetComponent<Projectile>();

                if (projectile != null)
                {
                    var speedMultiplier = actor.GetAnimationSpeedMultiplier();
                    
                    var projectileHit = false;
                    
                    projectile.Launch(spawnPos, targetPos, speedMultiplier, () =>
                    {
                        targetObj.GetComponent<Health>()?.TakeDamage(damage);
                        CameraFollow.Instance?.ShakeLight();
                        projectileHit = true;
                    });

                    while (!projectileHit && projectileObj != null)
                    {
                        yield return null;
                    }
                }
                else
                {
                    Debug.LogWarning("[RangedAttackAbility] Нет компонента Projectile");
                    targetObj.GetComponent<Health>()?.TakeDamage(damage);
                    CameraFollow.Instance?.ShakeMedium();
                }
            }
            else
            {
                targetObj.GetComponent<Health>()?.TakeDamage(damage);
                CameraFollow.Instance?.ShakeMedium();
                yield return new WaitForSeconds(actor.GetScaledTime(0.05f));
            }
        }
    }
}