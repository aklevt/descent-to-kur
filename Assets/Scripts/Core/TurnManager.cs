using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    [ContextMenu("Ход противника")]
    public void DebugEnemyTurn() => SetState(TurnState.EnemyTurn);

    private List<EnemyController> allEnemies = new();

    private IEnumerator EnemyTurnSequence()
    {
        var savedEnemiesList = new List<EnemyController>(allEnemies);
        foreach (var enemy in savedEnemiesList)
        {
            if (enemy == null)
                continue;
            yield return enemy.DoTurn();
        }

        yield return null;
        SetState(TurnState.PlayerTurn);
    }

    public void RegisterEnemy(EnemyController enemy) => allEnemies.Add(enemy);

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

    public void UnregisterEnemy(EnemyController enemy)
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