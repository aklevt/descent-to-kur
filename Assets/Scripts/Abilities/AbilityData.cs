using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public abstract class AbilityData : ScriptableObject
    {
        public string abilityName;
        public Color highlightColor = Color.white;
        public Color effectColor = new Color(1f, 0.2f, 0.2f, 0.9f);

        // Клетки для подсветки зоны выбора — от текущей позиции кастера
        public List<Vector3Int> GetTargetCells(BaseEntity actor)
            => GetTargetCellsFrom(actor.CurrentCell, actor);

        // Базовый метод — от произвольной позиции (нужен для превью)
        public abstract List<Vector3Int> GetTargetCellsFrom(Vector3Int position, BaseEntity actor);

        // Клетки эффекта при наведении на конкретную клетку
        public virtual List<Vector3Int> GetEffectCells(Vector3Int hoveredCell, BaseEntity caster)
            => new List<Vector3Int>();

        // Выполнить способность
        public abstract IEnumerator Execute(BaseEntity caster, Vector3Int targetCell);

        // Можно ли использовать прямо сейчас
        public virtual bool CanUse(BaseEntity caster) => true;
    }
}
