using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Abilities
{
    [CreateAssetMenu(fileName = "PlayerMove", menuName = "Abilities/PlayerMove")]
    public class PlayerMove : Move
    {
        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int position, Entity actor)
        {
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
            var player = (Player)actor;
            //player.SpendEnergy(distance * energyCost);
            actor.MoveToCell(targetCell);
            while (actor.IsMoving) yield return null;
        }
    }
}