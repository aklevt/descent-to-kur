using Sprites;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Базовый класс для всех врагов
/// Конкретные враги переопределяют ExecuteAction для своей логики
/// </summary>
public abstract class BaseEnemy : Entity
{
    [NonSerialized]
    public int MoveRange;

    protected override void Start()
    {
        base.Start();
        TurnManager.Instance?.RegisterEnemy(this);
    }

    protected virtual void OnDisable()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.UnregisterEnemy(this);
    }
    
    /// <summary>
    /// Выполнить полный ход врага: движение и действие
    /// </summary>
    public IEnumerator DoTurn()
    {
        if (TryGetComponent<Health>(out var h) && h.IsDead) yield break;
        
        Debug.Log($"{gameObject.name} готовится к ходу...");
        yield return new WaitForSeconds(0.1f);

        var bestMove = GetBestMove();

        if (bestMove.HasValue && bestMove.Value != CurrentCell)
        {
            MoveToCell(bestMove.Value);

            while (IsMoving)
            {
                yield return null;
            }
        }
        
        if (TryGetComponent<Health>(out var health) && health.IsDead) yield break;
        
        yield return new WaitForSeconds(0.1f);

        yield return ExecuteAction();
        
        yield return new WaitForSeconds(0.1f);
        Debug.Log($"{gameObject.name} закончил ход");
    }
    
    /// <summary>
    /// Вычисляет лучший ход в сторону игрока
    /// </summary>
    protected virtual Vector3Int? GetBestMove()
    {
        if (Player.Instance == null) return null;
        
        var playerCell = Player.Instance.CurrentCell;
        
        if (Vector3Int.Distance(CurrentCell, playerCell) <= 1.1f)
            return CurrentCell;
        
        // hard line (не обращайте внимания на этот комментарий)
        var possibleMoves = GridManager.Instance.GetWalkableTilesInRange(CurrentCell, MoveRange, gameObject);
    
        return possibleMoves
            .OrderBy(pos => Vector3.Distance(pos, playerCell))
            .Cast<Vector3Int?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Попытается использовать способность по индексу, если:
    /// 1. Индекс верный 
    /// 2. Способность можно использовать
    /// 3. Есть доступная цель
    /// </summary>
    protected IEnumerator TryUseAbility(int abilityIndex)
    {
        if (abilityIndex >= Abilities.Count) yield break;

        var ability = Abilities[abilityIndex];
        if (!ability.CanUse(this)) yield break;

        var target = ability.ChooseTarget(this);
        if (!target.HasValue) yield break;

        yield return StartCoroutine(ability.Execute(this, target.Value));
    }

    /// <summary>
    /// Реализуют конкретные враги для своего хода
    /// </summary>
    protected IEnumerator ExecuteAction()
    {
        yield return TryUseAbility(0);
    }
}