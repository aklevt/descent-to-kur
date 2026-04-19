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

        public override Vector3Int? ChooseTarget(BaseEntity actor)
        {
            var playerCell = PlayerMovement.Instance?.CurrentCell;
            if (playerCell == null) return null;

            var available = GetTargetCells(actor);
            return available.Contains(playerCell.Value) ? playerCell : null;
        }
        
        /// <summary>
        /// Атаковать ближней можем только если на клетке есть враг
        /// </summary>
        public override bool IsValidTarget(Vector3Int targetCell, BaseEntity caster)
        {
            var target = GridManager.Instance.GetEntityAt(targetCell);
            return target != null && target != caster.gameObject;
        }

        public override IEnumerator Execute(BaseEntity actor, Vector3Int targetCell)
        {
            var target = GridManager.Instance.GetEntityAt(targetCell);
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