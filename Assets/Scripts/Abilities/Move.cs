using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Abilities
{
    [CreateAssetMenu(fileName = "Move", menuName = "Abilities/Move")]
    public class Move : Ability
    {
        public int moveRange = 4;

        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int position, Entity actor)
        {
            // Грамотно переписать
            //var maxAvailableDistance = (actor is BaseEnemy) 
            //    ? moveRange
            //    : Mathf.Min(moveRange, actor.Energy);
            var player = (Player)actor;
            var maxAvailableDistance = Mathf.Min(moveRange, player.Energy / energyCost);

            if (maxAvailableDistance <= 0) return new List<Vector3Int>();
            
            return GridManager.Instance.GetWalkableTilesInRange(
                position,
                maxAvailableDistance,
                actor.gameObject
            );
        }

        public override IEnumerator Execute(Entity actor, Vector3Int targetCell)
        {
            var distance = Mathf.Abs(targetCell.x - actor.CurrentCell.x) + 
                           Mathf.Abs(targetCell.y - actor.CurrentCell.y);

            if (actor is Player)
            {
                var player = (Player)actor;
                player.SpendEnergy(distance * energyCost);
                //Stats.Energy = Mathf.Max(0, actor.Stats.Energy - distance);
                //SpendEnergy(distance);
            }
            
            actor.MoveToCell(targetCell);
            while (actor.IsMoving) yield return null;
        }
    }
}