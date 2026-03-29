using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    [ContextMenu("Ход противника")]
    public void DebugEnemyTurn() => SetState(TurnState.EnemyTurn);

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

    public void SetState(TurnState newState)
    {
        CurrentState = newState;
        
        Debug.Log($"<color=yellow>[TurnManager]</color> Ход сменился на: <b>{newState}</b>");

        OnStateChanged?.Invoke(newState);
    }

    public void EndPlayerTurn()
    {
        if (CurrentState == TurnState.PlayerTurn)
        {
            SetState(TurnState.EnemyTurn);
        }
    }
}