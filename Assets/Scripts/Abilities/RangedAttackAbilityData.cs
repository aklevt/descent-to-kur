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
            if (target == null) yield break;

            target.GetComponent<Health>()?.TakeDamage(actor.Stats.AttackDamage);
            CameraFollow.Instance?.ShakeMedium();

            yield return null;
        }
    }
}