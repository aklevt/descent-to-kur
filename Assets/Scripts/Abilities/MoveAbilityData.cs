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

        public override IEnumerator Execute(BaseEntity caster, Vector3Int targetCell)
        {
            caster.MoveToCell(targetCell);
            while (caster.IsMoving) yield return null;
        }
    }
}