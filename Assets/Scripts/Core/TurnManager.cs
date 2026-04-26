using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    [ContextMenu("Ход противника")]
    public void DebugEnemyTurn() => SetState(TurnState.EnemyTurn);

    private List<EnemyBase> allEnemies = new();
    
    public event Action OnAllEnemiesFrozen;

    private IEnumerator EnemyTurnSequence()
    {
        var savedEnemiesList = new List<EnemyBase>(allEnemies);
        var activeEnemies = savedEnemiesList
            .Where(e => e != null)
            .ToList();
        
        var allFrozen = activeEnemies.Count > 0 && activeEnemies.All(e => e.IsFreeze);
        
        if (allFrozen)
        {
            OnAllEnemiesFrozen?.Invoke();
        
            yield return new WaitForSeconds(1.0f);
            yield return ProcessEndOfRound();
        
            if (PlayerMovement.Instance.Stats.Health <= 0)
                yield break;
        
            SetState(TurnState.PlayerTurn);
            CameraFollow.Instance?.ResetFocus();
            yield break;
        }


        if (activeEnemies.Count > 0)
        {
            var center = activeEnemies
                             .Aggregate(Vector3.zero, (sum, e) => sum + e.transform.position)
                         / activeEnemies.Count;

            // Камера смещается к центру один раз на весь ход врагов
            CameraFollow.Instance?.ShiftTowards(center);
        }


        foreach (var enemy in savedEnemiesList)
        {
            if (enemy == null) continue;
            yield return enemy.DoTurn();
        }

        yield return ProcessEndOfRound();


        // yield return null;

        if (PlayerMovement.Instance.Stats.Health <= 0)
        {
            yield break;
        }

        SetState(TurnState.PlayerTurn);

        CameraFollow.Instance?.ResetFocus();
    }

    /// <summary>
    /// Обрабатывает все эффекты конца раунда (разморозка)
    /// </summary>
    private IEnumerator ProcessEndOfRound()
    {
        var allEntities = new List<BaseEntity>(allEnemies);
        if (PlayerMovement.Instance != null)
            allEntities.Add(PlayerMovement.Instance);

        foreach (var entity in allEntities)
        {
            if (entity != null)
                entity.OnRoundEnd();
        }

        foreach (var entity in allEntities)
        {
            if (entity != null && entity.Stats.Freeze == 0)
            {
                entity.UpdateVisualStatus();
                yield return new WaitForSeconds(0.02f);
            }
        }

        yield return new WaitForSeconds(0.1f);
    }

    public void RegisterEnemy(EnemyBase enemy)
    {
        if (!allEnemies.Contains(enemy))
        {
            allEnemies.Add(enemy);
        }
    }

    public static TurnManager Instance { get; private set; }

    public TurnState CurrentState { get; private set; }

    public event Action<TurnState> OnStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnregisterEnemy(EnemyBase enemy)
    {
        if (allEnemies.Contains(enemy))
        {
            allEnemies.Remove(enemy);
            Debug.Log($"<color=red>[TurnManager]</color> Противник удален. Осталось: {allEnemies.Count}");
        }
    }

    private void SetState(TurnState newState)
    {
        CurrentState = newState;

        if (newState == TurnState.PlayerTurn && PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.OnTurnStart();
            PlayerMovement.Instance.Stats.RestoreEnergy(PlayerMovement.Instance.Stats.MaxEnergy);
        }

        Debug.Log($"<color=yellow>[TurnManager]</color> Ход сменился на: <b>{newState}</b>");

        OnStateChanged?.Invoke(newState);

        if (newState == TurnState.EnemyTurn)
        {
            StartCoroutine(EnemyTurnSequence());
        }
    }

    public void BeginLevel()
    {
        SetState(TurnState.PlayerTurn);
    }

    public void EndPlayerTurn()
    {
        if (CurrentState == TurnState.PlayerTurn)
        {
            SetState(TurnState.EnemyTurn);
        }
    }

    public void ResetEnemies()
    {
        allEnemies.Clear();
        StopAllCoroutines(); // ❗ Это временно, надо переписать ❗
    }
}