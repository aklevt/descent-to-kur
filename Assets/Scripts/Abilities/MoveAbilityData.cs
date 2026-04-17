using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "MoveAbility", menuName = "Abilities/Move")]
    public class MoveAbilityData : AbilityData
    {
        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int position, BaseEntity actor)
        {
            return GridManager.Instance.GetWalkableTilesInRange(
                position,
                actor.Stats.MoveRange,
                actor.gameObject
            );
        }
        
        public override List<Vector3Int> GetEffectCells(Vector3Int hoveredCell, BaseEntity caster)
            => new List<Vector3Int> { hoveredCell };

        public override IEnumerator Execute(BaseEntity actor, Vector3Int targetCell)
        {
            actor.MoveToCell(targetCell);
            while (actor.IsMoving) yield return null;
        }
    }
}