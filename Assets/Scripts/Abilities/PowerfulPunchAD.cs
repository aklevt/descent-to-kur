using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "PowerfulPunchAbility", menuName = "Abilities/PowerfulPunch")]
    public class PowerfulPunchAD : AbilityData
    {
        public override List<Vector3Int> GetTargetCellsFrom(Vector3Int origin, BaseEntity caster)
        {
            return GridManager.Instance.GetAttackableCellsInRadius(origin, 1);
        }

        public override List<Vector3Int> GetEffectCells(Vector3Int hoveredCell, BaseEntity actor)
        {
            var effectCells = new List<Vector3Int>();
            Vector3Int rayVector;
            Vector3Int deltaVector;
            if (hoveredCell - actor.CurrentCell == Vector3Int.up)
            {
                rayVector = Vector3Int.up;
                deltaVector = Vector3Int.right;
            }
            else if (hoveredCell - actor.CurrentCell == Vector3Int.down)
            {
                rayVector = Vector3Int.down;
                deltaVector = Vector3Int.right;
            }
            else if (hoveredCell - actor.CurrentCell == Vector3Int.right)
            {
                rayVector = Vector3Int.right;
                deltaVector = Vector3Int.up;
            }
            else if (hoveredCell - actor.CurrentCell == Vector3Int.left)
            {
                rayVector = Vector3Int.left;
                deltaVector = Vector3Int.up;
            }
            else
            {
                return effectCells;
            }
            for (var j = 2; j > 0; j--)
            {
                for (var i = -1; i <= 1; i++)
                {
                    effectCells.Add(actor.CurrentCell + j * rayVector + i * deltaVector);
                }
            }
            return effectCells;
            
        }

        public override bool IsValidTarget(Vector3Int targetCell, BaseEntity caster)
        {
            foreach (var cell in GetEffectCells(targetCell, caster))
            {
                var target = GridManager.Instance.GetEntityAt(cell);
                if (target != null)
                    return true;
            }
            return false;
        }

        public override IEnumerator Execute(BaseEntity actor, Vector3Int targetCell)
        {
            CameraFollow.Instance?.ShakeMedium();
            var knobackVector = targetCell - actor.CurrentCell;
            foreach (var cell in GetEffectCells(targetCell, actor))
            {
                var target = GridManager.Instance.GetEntityAt(cell);
                if (target != null)
                {
                    var targetHealth = target.GetComponent<Health>();
                    KnobackEnemy(knobackVector, target.GetComponent<BaseEntity>());
                    targetHealth?.TakeDamage(actor.Stats.AttackDamage);
                    yield return null;
                }
            }
        }

        private void KnobackEnemy(Vector3Int direction, BaseEntity target)
        {
            var canKnoback = GridManager.Instance.IsCellWalkable(target.CurrentCell + direction);
            var canFarKnoback = GridManager.Instance.IsCellWalkable(target.CurrentCell + 2 * direction);
            if (canKnoback && canFarKnoback)
                target.MoveToCell(target.CurrentCell + 2 * direction);
            else if (canKnoback)
                target.MoveToCell(target.CurrentCell + direction);

        }
    }
}