using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// Базовый класс для всех способностей в игре. 
    /// Отвечает за логику поиска целей, визуализацию и исполнение способности
    /// </summary>
    public abstract class AbilityData : ScriptableObject
    {
        public string abilityName;
        
        [Header("Cost")]
        public int energyCost = 0;
        
        [Header("Colors")]
        public Color highlightColor = Color.white;
        public Color effectColor = new Color(1f, 0.2f, 0.2f, 0.9f);

        /// <summary>
        /// Вычисляет доступные для выбора клетки по текущему положению исполнителя
        /// </summary>
        /// <param name="actor">Сущность, использующая способность</param>
        /// <returns>Список координат клеток, на которые можно нажать для активации способности</returns>
        public List<Vector3Int> GetTargetCells(BaseEntity actor)
            => GetTargetCellsFrom(actor.CurrentCell, actor);

        /// <summary>
        /// Вычисляет доступные клетки от заданной точки (нужен для превью)
        /// </summary>
        /// /// <param name="position">Точка отсчета</param>
        /// <param name="actor">Сущность, применяющая способность (учитывает её stats)</param>
        /// <returns>Список координат доступных клеток</returns>
        public abstract List<Vector3Int> GetTargetCellsFrom(Vector3Int position, BaseEntity actor);

        /// <summary>
        /// Определяет область непосредственного воздействия способности
        /// </summary>
        /// <param name="hoveredCell">Клетка, над которой находится курсор</param>
        /// <param name="actor">Сущность, применяющая способность</param>
        /// <returns>Список клеток, которые будут подсвечены как "зона поражения"</returns>
        public virtual List<Vector3Int> GetEffectCells(Vector3Int hoveredCell, BaseEntity actor)
            => new();

        /// <summary>
        /// Исполнение логики способности (анимации, нанесение урона, перемещение)
        /// </summary>
        /// <param name="actor">Сущность, применяющая способность</param>
        /// <param name="targetCell">Выбранная целевая клетка</param>
        /// <returns></returns>
        public abstract IEnumerator Execute(BaseEntity actor, Vector3Int targetCell);

        /// <summary>
        /// Проверка условий активации способности (наличие энергии, кулдаун, состояние actor).
        /// </summary>
        /// <param name="user">Сущность, для которой надо вызвать проверку</param>
        /// <returns>True, если можно использовать способность</returns>
        public virtual bool CanUse(BaseEntity user) => true;
        
        /// <summary>
        /// Выбор цели через AI
        /// </summary>
        /// <param name="actor">Враг или союзник (??)</param>
        /// <returns>Координата цели или null</returns>
        public virtual Vector3Int? ChooseTarget(BaseEntity actor)
        {
            var cells = GetTargetCells(actor);
            return cells.Count > 0 ? cells[0] : null;
        }
        
        /// <summary>
        /// Проверяет, можно ли выполнить способность на этой клетке (переопределяется для обычной атаки)
        /// Если вернет false, клик на клетку будет проигнорирован
        /// </summary>
        public virtual bool IsValidTarget(Vector3Int targetCell, BaseEntity caster)
        {
            return true;
        }
    }
}
