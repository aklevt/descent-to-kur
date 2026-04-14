using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sprites;
using UnityEngine.UI;

public enum AbilityType
{
    None,
    Move,
    Attack,
    RangedAttack,
}

public class AbilityController : MonoBehaviour
{
    public static AbilityController Instance { get; private set; }

    [Header("UI Elements")] [SerializeField]
    private Button moveButton;

    [SerializeField] private Button attackButton;

    private AbilityType selectedAbility = AbilityType.Move;
    private readonly List<Vector3Int> availableCells = new();

    /// <summary>
    /// Разрешено ли взаимодействие со способностями в текущем состоянии игры
    /// </summary>
    private bool IsPlayerTurnActive =>
        !isDead &&
        TurnManager.Instance != null &&
        TurnManager.Instance.CurrentState == TurnState.PlayerTurn &&
        PlayerMovement.Instance != null;

    /// <summary>
    /// Список координат клеток, доступных для текущей выбранной способности
    /// </summary>
    public List<Vector3Int> AvailableCells => availableCells;

    private bool isDead = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged += HandleTurnChanged;
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged -= HandleTurnChanged;
    }

    private void Update()
    {
        if (!IsPlayerTurnActive) return;

        // Автоматическое обновление сетки после завершения движения
        if (ShouldRefreshAbilityGrid())
        {
            RefreshAbilityOverlay();
        }
    }

    /// <summary>
    /// Проверка условий, когда нужно перерисовать сетку:
    /// 1. Ход игрока
    /// 2. Игрок физически остановился
    /// 3. Сетка в данный момент пуста, так как была очищена
    /// </summary>
    private bool ShouldRefreshAbilityGrid()
    {
        return !PlayerMovement.Instance.IsMoving &&
               selectedAbility != AbilityType.None &&
               availableCells.Count == 0;
    }

    private void HandleTurnChanged(TurnState newState)
    {
        if (newState == TurnState.PlayerTurn)
            SelectAbility((int)AbilityType.Move);
        else
        {
            ClearSelection();
            UpdateButtonsState();
        }
    }

    /// <summary>
    /// Обрабатывает нажатие игрока на клетку игрового поля
    /// </summary>
    public void HandleCellClick(Vector3Int clickedCell)
    {
        if (!IsPlayerTurnActive || PlayerMovement.Instance.IsMoving)
            return;

        if (availableCells.Contains(clickedCell))
        {
            ExecuteSelectedAbility(clickedCell);
        }
    }

    private void ExecuteSelectedAbility(Vector3Int targetCell)
    {
        switch (selectedAbility)
        {
            case AbilityType.Move:
                PerformMovement(targetCell);
                break;

            case AbilityType.Attack:
                PerformAttack(targetCell);
                break;

            case AbilityType.RangedAttack:
                PerformRangedAttack(targetCell);
                break;

            case AbilityType.None:
                break;
        }
    }

    private void PerformRangedAttack(Vector3Int targetCell)
    {
        var target = GridManager.Instance.GetEntityAt(targetCell);
        if (target == null) return;
        
        if (target.TryGetComponent<Health>(out var targetHealth))
        {
            var damage = PlayerMovement.Instance.Stats.AttackDamage;
            targetHealth.TakeDamage(damage);
        }
        
        SelectAbility((int)AbilityType.None);
    }

    private void PerformAttack(Vector3Int targetCell)
    {
        var target = GridManager.Instance.GetEntityAt(targetCell);

        if (target != null && target != PlayerMovement.Instance.gameObject)
        {
            StartCoroutine(PlayerAttackSequence(target));
        }
        else
        {
            Debug.Log("На клетке нет противника");
            SelectAbility((int)AbilityType.None);
        }
    }

    private IEnumerator PlayerAttackSequence(GameObject target)
    {
        var targetHealth = target.GetComponent<Health>();
        var damage = PlayerMovement.Instance.Stats.AttackDamage;

        yield return StartCoroutine(PlayerMovement.Instance.PunchAnimation(
            target.transform.position,
            () =>
            {
                if (targetHealth != null)
                    targetHealth.TakeDamage(damage);
            }
        ));

        SelectAbility((int)AbilityType.None);
    }

    private void PerformMovement(Vector3Int targetCell)
    {
        PlayerMovement.Instance.ExecuteMove(targetCell);
        ClearSelection();
    }

    private void RefreshAbilityOverlay()
    {
        ClearSelection();
        UpdateButtonsState();

        if (!IsPlayerTurnActive) return;

        var playerCell = PlayerMovement.Instance.CurrentCell;

        switch (selectedAbility)
        {
            case AbilityType.Move:
                PrepareMoveArea(playerCell);
                RenderSelection(Color.white);
                break;

            case AbilityType.Attack:
                PrepareAttackArea(playerCell);
                RenderSelection(Color.red);
                break;

            case AbilityType.RangedAttack:
                PrepareRangeAttackArea(playerCell);
                RenderSelection(Color.red);
                break;
        }
    }

    /// <summary>
    /// Заполняет список клеток, доступных для атаки
    /// </summary>
    private void PrepareAttackArea(Vector3Int playerCell)
    {
        var attackable = GridManager.Instance.GetAttackableCellsInRadius(playerCell, 1);
        availableCells.AddRange(attackable);
    }

    private void PrepareRangeAttackArea(Vector3Int playerCell)
    {
        var attackable = GridManager.Instance.GetAttackableCellsInRadius(playerCell, 3, 2);

        availableCells.AddRange(attackable);
    }

    /// <summary>
    /// Заполняет список доступных клеток для перемещения игрока
    /// </summary>
    private void PrepareMoveArea(Vector3Int playerCell)
    {
        var walkable = GridManager.Instance.GetWalkableTilesInRange(
            playerCell,
            PlayerMovement.Instance.Stats.MoveRange,
            PlayerMovement.Instance.gameObject
        );
        availableCells.AddRange(walkable);
    }

    private void RenderSelection(Color color)
    {
        if (GridHighlighter.Instance != null && availableCells.Count > 0)
        {
            GridHighlighter.Instance.HighlightCells(availableCells, color);
        }
    }

    private void ClearSelection()
    {
        availableCells.Clear();
        GridHighlighter.Instance.Clear();
    }

    /// <summary>
    /// Переключает текущую активную способность, вызывается кнопками UI
    /// </summary>
    private void SelectAbility(int abilityIndex)
    {
        var newAbility = (AbilityType)abilityIndex;
        if (selectedAbility == newAbility) return;

        selectedAbility = newAbility;
        RefreshAbilityOverlay();
    }

    /// <summary>
    /// Управляет доступностью кнопок в зависимости от выбранного режима
    /// </summary>
    private void UpdateButtonsState()
    {
        if (!IsPlayerTurnActive)
        {
            if (moveButton != null) moveButton.interactable = false;
            if (attackButton != null) attackButton.interactable = false;
            return;
        }

        if (moveButton != null)
            moveButton.interactable = (selectedAbility != AbilityType.Move);

        if (attackButton != null)
            attackButton.interactable = (selectedAbility != AbilityType.Attack);
    }

    public void DisableAllOverlaysAfterDeath()
    {
        isDead = true;
        selectedAbility = AbilityType.None;
        ClearSelection();
        UpdateButtonsState();
    }

    public void SetMoveMode() => SelectAbility((int)AbilityType.Move);
    public void SetAttackMode() => SelectAbility((int)AbilityType.Attack);
    public void SetRangedAttackMode() => SelectAbility((int)AbilityType.RangedAttack);
}