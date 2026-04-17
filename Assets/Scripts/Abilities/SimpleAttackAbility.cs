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
        
        public override Vector3Int? ChooseTarget(BaseEntity actor)
        {
            var playerCell = PlayerMovement.Instance?.CurrentCell;
            if (playerCell == null) return null;

            var available = GetTargetCells(actor);
            return available.Contains(playerCell.Value) ? playerCell : null;
        }

        public override IEnumerator Execute(BaseEntity actor, Vector3Int targetCell)
        {
            var target = GridManager.Instance.GetEntityAt(targetCell);
            if (target == null || target == actor.gameObject) yield break;

            var targetHealth = target.GetComponent<Health>();

            yield return actor.StartCoroutine(actor.PunchAnimation(
                target.transform.position,
                () =>
                {
                    targetHealth?.TakeDamage(actor.Stats.AttackDamage);
                    CameraFollow.Instance?.ShakeMedium();
                }
            ));
        }
    }
}