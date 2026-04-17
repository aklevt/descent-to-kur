using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "SimpleAttackAbility", menuName = "Abilities/SimpleAttack")]
    public class SimpleAttackAbilityData : AbilityData
    {
        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int origin, BaseEntity caster)
        {
            return GridManager.Instance.GetAttackableCellsInRadius(origin, 1);
        }

        public override List<Vector3Int> GetEffectCells(Vector3Int hoveredCell, BaseEntity caster)
            => new List<Vector3Int> { hoveredCell };

        public override IEnumerator Execute(BaseEntity caster, Vector3Int targetCell)
        {
            var target = GridManager.Instance.GetEntityAt(targetCell);
            if (target == null || target == caster.gameObject) yield break;

            var targetHealth = target.GetComponent<Health>();

            yield return caster.StartCoroutine(caster.PunchAnimation(
                target.transform.position,
                () =>
                {
                    targetHealth?.TakeDamage(caster.Stats.AttackDamage);
                    CameraFollow.Instance?.ShakeMedium();
                }
            ));
        }
    }
}