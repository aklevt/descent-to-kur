using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "MoveAbility", menuName = "Abilities/Move")]
    public class MoveAbilityData : AbilityData
    {
        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int position, BaseEntity actor)
        {
            int maxAvailableDistance;
            
            if (actor is PlayerMovement player)
            {
                maxAvailableDistance = Mathf.Min(player.Stats.RemainingSteps, player.Stats.Energy);
            }
            else if (actor is EnemyBase)
            {
                maxAvailableDistance = actor.Stats.MoveRange;
            }
            else
            {
                maxAvailableDistance = 0;
            }

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

            if (actor is PlayerMovement player)
            {
                if (!player.Stats.CanMove(distance) || !player.Stats.HasEnergyForAction(distance))
                {
                    if (!player.Stats.HasEnergyForAction(distance))
                        UI.UIController.Instance?.ShowEnergyWarning();
                    else
                        UI.UIController.Instance?.ShowStepsWarning();
                    
                    yield break;
                }

                player.Stats.SpendEnergy(distance);
                player.Stats.SpendSteps(distance);
                
                Debug.Log($"<color=cyan>[MoveAbility]</color> Потрачено: {distance}. Осталось энергии: {player.Stats.Energy}, шагов: {player.Stats.RemainingSteps}");
            }
            
            actor.MoveToCell(targetCell);
            while (actor.IsMoving) yield return null;
            
            if (actor is PlayerMovement player2)
            {
                var hasStepsNow = player2.Stats.RemainingSteps > 0;
                var hasEnergyNow = player2.Stats.Energy > 0;
                
                if (!hasStepsNow && hasEnergyNow)
                {
                    Debug.Log("<color=yellow>[MoveAbility]</color> Доступные шаги закончились");
                    
                    yield return new WaitForSeconds(player2.GetScaledTime(0.1f));
                    
                    UI.UIController.Instance?.ShowStepsEndedWarning();
                }
            }
        }
    }
}