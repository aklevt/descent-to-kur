using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "RangedAttackAbility", menuName = "Abilities/RangedAttack")]
    public class RangedAttackAbilityData : AbilityData
    {
        public int minRange = 2;
        public int maxRange = 3;

        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int origin, BaseEntity caster)
        {
            return GridManager.Instance.GetAttackableCellsInRadius(origin, maxRange, minRange);
        }

        public override List<Vector3Int> GetEffectCells(Vector3Int hoveredCell, BaseEntity caster)
            => new List<Vector3Int> { hoveredCell };

        public override IEnumerator Execute(BaseEntity caster, Vector3Int targetCell)
        {
            var target = GridManager.Instance.GetEntityAt(targetCell);
            if (target == null) yield break;

            target.GetComponent<Health>()?.TakeDamage(caster.Stats.AttackDamage);
            CameraFollow.Instance?.ShakeMedium();

            yield return null;
        }
    }
}