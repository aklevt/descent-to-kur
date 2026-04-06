using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using static UnityEngine.GraphicsBuffer;

public class AbilityController : MonoBehaviour
{
    // Объекты
    public static AbilityController Instance;
    

    // Игровые ???
    public string SelectedAbility { get; private set; }
    public List<Vector3Int> AvailableCells { get; private set; } = new();
    public Vector3Int PlayerPosition { get; private set; }
    
    // Характеристики способностей:
    // Передвижение
    [SerializeField] private int moveRange = 1;

    private void Awake()
    {
        Instance = this;
    }
    private void HandleTurnChanged(TurnState newState)
    {
        ChangeAbility(null);
        /*isPlayerTurn = (newState == TurnState.PlayerTurn);
        // Debug.Log(isPlayerTurn);
        if (isPlayerTurn)
        {
            UpdateAvailableMoves();
        }
        else
        {
            GridHighlighter.Instance.Clear();
        }*/
    }

    private void Start()
    {
        var startCell = GridManager.Instance.WorldPointToCell(transform.position);
        GridManager.Instance.MoveEntity(PlayerPosition, startCell, gameObject);
        PlayerPosition = startCell;
        //UpdateTargetPosition(logicalCellPos);

        //if (isPlayerTurn) UpdateAvailableMoves();
        TurnManager.Instance.OnStateChanged += HandleTurnChanged;
        ChangeAbility(null);

        //if (TurnManager.Instance != null)
        //{
        //    HandleTurnChanged(TurnManager.Instance.CurrentState);
        //}
    }

    public void ChangeAbility(string newAbility)
    {
        SelectedAbility = newAbility;
        UpdateAvailableCells();
    }

    private void UpdateAvailableCells()
    {
        AvailableCells.Clear();
        GridHighlighter.Instance.Clear();
        if (SelectedAbility == "Move")
            UpdateAbilityAvailableCells(GridManager.Instance.GetWalkableTilesInRange);  //, gameObject);]
        else if (SelectedAbility == "Punch")
            UpdateAbilityAvailableCells(GridManager.Instance.GetBeatableCells);
    }

    private void UpdateAbilityAvailableCells(Func<Vector3Int, int, List<Vector3Int>> AvailableMovesGetter)
    {
        AvailableCells = AvailableMovesGetter(PlayerPosition, moveRange);
        GridHighlighter.Instance.HighlightCells(AvailableCells);
    }

    public void DoAction(Vector3Int selectedCell)
    {
        if (SelectedAbility == "Move")
        {
            MoveAction(selectedCell);
        }   
        else if (SelectedAbility == "Punch")
            PunchAction(selectedCell);
    }

    private void MoveAction(Vector3Int target)
    {
        GridHighlighter.Instance.Clear();
        GridManager.Instance.MoveEntity(PlayerPosition, target, gameObject);
        PlayerPosition = target;
        //UpdateTargetPosition(logicalCellPos);
        //lastStepTime = Time.time;
        UpdateAvailableCells();
    }

    private void PunchAction(Vector3Int target)
    {
        //GridManager.Instance.entitiesOnGrid[target]
        Debug.Log("Вообразим, что прописал двоечку");
    }
}
