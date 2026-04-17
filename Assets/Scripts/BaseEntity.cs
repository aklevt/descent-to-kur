using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Stats;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    [Header("Abilities")]
    [SerializeField] private List<AbilityData> abilities = new();
    
    public IReadOnlyList<AbilityData> Abilities => abilities;
    
    [Header("Stats")] [SerializeField] private EntityStats baseStats;

    [Header("Live Stats (Edit here if Customized)")] [SerializeField]
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
        CurrentCell = GridManager.Instance.WorldToCell(transform.position);
        GridManager.Instance.RegisterFixedEntity(CurrentCell, gameObject);

        PlaceOnCell();
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

    private void SetLogicalPosition(Vector3Int newCell)
    {
        GridManager.Instance.MoveEntity(CurrentCell, newCell, gameObject);
        CurrentCell = newCell;
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


    public IEnumerator PunchAnimation(Vector3 targetPos, Action onHit = null)
    {
        var startPos = transform.position;
        var punchPos = Vector3.Lerp(startPos, targetPos, 0.3f);
        var duration = 0.15f;
        var elapsed = 0f;

        FlipToTarget(targetPos);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, punchPos, elapsed / duration);
            yield return null;
        }

        transform.position = punchPos;

        onHit?.Invoke();

        yield return new WaitForSeconds(0.05f);

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
    
    /// <summary>
    /// Выполнить способность по индексу
    /// </summary>
    public IEnumerator UseAbility(int index, Vector3Int targetCell)
    {
        if (index < 0 || index >= abilities.Count) yield break;
        if (!abilities[index].CanUse(this)) yield break;
        yield return StartCoroutine(abilities[index].Execute(this, targetCell));
    }

    protected virtual void OnArrivingToTarget()
    {
    }
}