using System.Collections;
using System.Linq;
using Sprites;
using UnityEngine;

public abstract class EnemyBase : BaseEntity
{
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

    protected virtual Vector3Int? GetBestMove()
    {
        if (PlayerMovement.Instance == null) return null;
        
        var playerCell = PlayerMovement.Instance.CurrentCell;
        
        if (Vector3Int.Distance(CurrentCell, playerCell) <= 1.1f)
            return CurrentCell;
        
        var possibleMoves = GridManager.Instance.GetWalkableTilesInRange(CurrentCell, Stats.MoveRange, gameObject);
    
        return possibleMoves
            .OrderBy(pos => Vector3.Distance(pos, playerCell))
            .Cast<Vector3Int?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Попытаться использовать способность по индексу
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
    protected abstract IEnumerator ExecuteAction();


    
}