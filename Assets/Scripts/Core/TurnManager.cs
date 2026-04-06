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
        foreach (var enemy in allEnemies)
        {
            //???
            if (enemy == null) 
                continue;
            yield return enemy.DoTurn();
        }
        
        yield return null;
        SetState(TurnState.PlayerTurn);
    }

    //?
    public void RegisterEnemy(EnemyController enemy) => allEnemies.Add(enemy);

    public static TurnManager Instance { get; private set; }

    public TurnState CurrentState { get; private set; }

    // Действия при смене State?
    public event Action<TurnState> OnStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {   
            // ???
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetState(TurnState.PlayerTurn);
    }

    private void SetState(TurnState newState)
    {
        CurrentState = newState;
        Debug.Log($"<color=yellow>[TurnManager]</color> Ход сменился на: <b>{newState}</b>");

        // Выключить кнопки?
        OnStateChanged?.Invoke(newState);

        if (newState == TurnState.EnemyTurn)
        {
            // Выполнять действие параллельно
            StartCoroutine(EnemyTurnSequence());
        }
    }

    public void BeginLevel() 
    {
        SetState(TurnState.PlayerTurn);
    }

    /// <summary>
    /// Метод вызывается, когда игрок жмет на кнопку "Завершить ход"
    /// </summary>
    public void EndPlayerTurn()
    {
        if (CurrentState == TurnState.PlayerTurn)
        {
            SetState(TurnState.EnemyTurn);
        }
    }
}