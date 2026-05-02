using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "RangedAttackAbility", menuName = "Abilities/RangedAttack")]
    public class RangedAttackAbilityData1 : Ability
    {
        public int MinRange = 3;
        public int MaxRange = 5;
        public int Damage = 10;

        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int origin, Entity actor)
        {
            return GridManager.Instance.GetAttackableCellsInRadius(origin, MaxRange, MinRange);
        }

        public override List<Vector3Int> GetEffectCells(Vector3Int hoveredCell, Entity actor)
            => new List<Vector3Int> { hoveredCell };
        
        public override Vector3Int? ChooseTarget(Entity actor)
        {
            var playerCell = Player.Instance?.CurrentCell;
            if (playerCell == null) return null;

            var available = GetTargetCells(actor);
            return available.Contains(playerCell.Value) ? playerCell : null;
        }
        
        /// <summary>
        /// Требует наличие цели на клетке, так как это не aoe-атака
        /// </summary>
        public override bool IsValidTarget(Vector3Int targetCell, Entity actor)
        {
            var target = GridManager.Instance.GetEntityAt(targetCell);
            return target != null;
        }
        
        public override IEnumerator Execute(Entity actor, Vector3Int targetCell)
        {
            var target = GridManager.Instance.GetEntityAt(targetCell);
            target.GetComponent<Health>()?.TakeDamage(Damage);
            CameraFollow.Instance?.ShakeMedium();

            yield return null;
        }
    }
}