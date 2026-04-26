using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Stats;
using UnityEngine;

/// <summary>
/// Базовый класс для всех персонажей (игрок, враги)
/// Наследуемые классы отвечают за все, что связано с сущностью: позиция на сетке, движение, анимация и список способностей
/// </summary>
public abstract class BaseEntity : MonoBehaviour
{
    [Header("Abilities")] [SerializeField] private List<AbilityData> abilities = new();

    public IReadOnlyList<AbilityData> Abilities => abilities;

    [Header("Stats")] [SerializeField] private EntityStats baseStats;

    [Header("Live Stats")] [SerializeField]
    private EntityRuntimeStats stats = new();

    public EntityRuntimeStats Stats => stats;

    [Header("Components")] [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField] protected Animator animator;

    public Vector3Int CurrentCell { get; private set; }
    public bool IsMoving { get; private set; }

    private Vector3 targetWorldPos;

    protected virtual void Start()
    {
    }

    public void InitializeOnGrid()
    {
        stats.Health = stats.MaxHealth;
        stats.Freeze = 0; 
        UpdateVisualStatus();

        CurrentCell = GridManager.Instance.WorldToCell(transform.position);
        PlaceOnCell();
        IsMoving = false;

        GridManager.Instance.RegisterFixedEntity(CurrentCell, gameObject);
    }


    public bool OnTurnStart()
    {
        if (stats.Freeze > 0)
        {
            Debug.Log($"<color=cyan>{name}</color> пропускает ход. Осталось: {stats.Freeze}");
            return false;
        }
    
        return true;
    }

    public void OnRoundEnd()
    {
        var wasFreezed = stats.Freeze > 0;
    
        if (stats.Freeze > 0)
        {
            stats.Freeze--;
        
            if (stats.Freeze == 0)
            {
                Debug.Log($"<color=cyan>{name}</color> разморозился");
            }
        }
    }

    public void Freeze(int turns)
    {
        stats.Freeze = Mathf.Max(stats.Freeze, turns);
        UpdateVisualStatus();
        Debug.Log($"<color=cyan>{name}</color> заморожен на {stats.Freeze} ходов!");
    }

    public bool IsFreeze => stats.Freeze > 0;

    public void UpdateVisualStatus()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = IsFreeze ? new Color(0.5f, 0.7f, 1f) : Color.white;
    }

    private void OnValidate()
    {
        if (!stats.isCustomized && baseStats != null)
        {
            stats.Initialize(baseStats);
        }
    }

    private void Update()
    {
        MoveSmoothly();
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Установка статов на свякий случай, пусть будет
        if (baseStats != null && !stats.isCustomized)
        {
            stats.Initialize(baseStats);
        }

        // Намеренно продублирована логика. Даже если в инспекторе изменить здоровье (isCustomized == true), оно сбросится после респауна
        stats.Health = stats.MaxHealth;
    }

    private void PlaceOnCell()
    {
        UpdateTargetPosition(CurrentCell);
        transform.position = targetWorldPos;
    }

    /// <summary>
    /// Выполняет мгновенный телепорт сущности в указанную клетку сетки
    /// Пере-регистрирует сущность в <see cref="GridManager"/>, сбрасывает состояние движения
    /// </summary>
    /// <param name="targetCell">Координаты целевой клетки на сетке</param>
    public void TeleportToCell(Vector3Int targetCell)
    {
        GridManager.Instance.UnregisterEntity(CurrentCell);
        CurrentCell = targetCell;
        IsMoving = false;

        PlaceOnCell();
        GridManager.Instance.RegisterFixedEntity(CurrentCell, gameObject);
    }

    public void MoveToCell(Vector3Int targetCell)
    {
        FlipToTarget(targetCell);
        GridManager.Instance.MoveEntity(CurrentCell, targetCell, gameObject);
        CurrentCell = targetCell;

        targetWorldPos = GridManager.Instance.GetCellCenterWorld(targetCell);
        targetWorldPos.z = transform.position.z;
        IsMoving = true;
    }

    public void MoveSmoothly()
    {
        if (!IsMoving) return;

        var dt = Mathf.Min(Time.deltaTime, 0.03f);
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, Stats.MoveSpeed * dt);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.001f)
        {
            transform.position = targetWorldPos;
            IsMoving = false;

            OnArrivingToTarget();
        }
    }

    /// <summary>
    /// Простейшая анимация удара с возможностью вызова действия в сам момент удара
    /// </summary>
    public IEnumerator PunchAnimation(Vector3 targetPos, Action onHit = null)
    {
        var startPos = transform.position;
        var punchPos = Vector3.Lerp(startPos, targetPos, 0.3f);
        var duration = 0.15f;
        var elapsed = 0f;

        FlipToTarget(targetPos);

        // Удар вперед
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, punchPos, elapsed / duration);
            yield return null;
        }

        transform.position = punchPos;

        onHit?.Invoke();

        yield return new WaitForSeconds(0.05f);

        // Возврат назад
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(punchPos, startPos, elapsed / duration);
            yield return null;
        }

        transform.position = startPos;
    }

    private void UpdateSpriteFlip(float horizontalDirection)
    {
        if (horizontalDirection != 0)
            spriteRenderer.flipX = horizontalDirection < 0;
    }

    public void FlipToTarget(Vector3 targetCell)
    {
        var directionX = targetCell.x - CurrentCell.x;
        UpdateSpriteFlip(directionX);
    }

    private void UpdateTargetPosition(Vector3Int cell)
    {
        targetWorldPos = GridManager.Instance.GetCellCenterWorld(cell);
        targetWorldPos.z = transform.position.z;
    }

    protected virtual void OnArrivingToTarget()
    {
    }
}