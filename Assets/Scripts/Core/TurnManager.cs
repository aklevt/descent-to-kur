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

    private IEnumerator EnemyTurnSequence()
    {
        var savedEnemiesList = new List<EnemyBase>(allEnemies);
        var activeEnemies = savedEnemiesList.Where(e => e != null).ToList();
        
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
            if (enemy == null)
                continue;
            yield return enemy.DoTurn();
        }
        
        CameraFollow.Instance?.ResetFocus();

        yield return null;
        SetState(TurnState.PlayerTurn);
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
}