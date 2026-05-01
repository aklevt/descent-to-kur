using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "MoveAbility", menuName = "Abilities/Move")]
    public class MoveAbilityData : AbilityData
    {
        public int moveRange = 4;

        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int position, BaseEntity actor)
        {
            var maxAvailableDistance = (actor is EnemyBase) 
                ? moveRange
                : Mathf.Min(moveRange, actor.Stats.Energy);

            if (maxAvailableDistance <= 0) return new List<Vector3Int>();
            
            return GridManager.Instance.GetWalkableTilesInRange(
                position,
                maxAvailableDistance,
                actor.gameObject
            );
        }

        public override IEnumerator Execute(BaseEntity actor, Vector3Int targetCell)
        {
            var distance = Mathf.Abs(targetCell.x - actor.CurrentCell.x) + 
                           Mathf.Abs(targetCell.y - actor.CurrentCell.y);

            if (actor is PlayerMovement)
            {
                actor.Stats.SpendEnergy(distance);
            }
            
            actor.MoveToCell(targetCell);
            while (actor.IsMoving) yield return null;
        }
    }
}