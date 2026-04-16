using System;
using System.Collections;
using Stats;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
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
        var duration = 0.15f;
        var elapsed = 0f;

        var isVertical = IsVerticalAttack(startPos, targetPos);
        var preAttackPos = startPos;

        if (isVertical)
        {
            var facingRight = !spriteRenderer.flipX;

            var diagOffset = GetDiagonalOffset(startPos, targetPos, facingRight);
            preAttackPos = startPos + diagOffset;

            UpdateSpriteFlip(diagOffset.x);

            elapsed = 0f;
            while (elapsed < duration * 0.8f)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, preAttackPos, elapsed / (duration * 0.8f));
                yield return null;
            }

            transform.position = preAttackPos;
            var dirToTarget = targetPos.x - preAttackPos.x;
            UpdateSpriteFlip(dirToTarget);
        }
        else
        {
            FlipToTarget(targetPos);
        }

        // выпад
        var punchPos = Vector3.Lerp(preAttackPos, targetPos, 0.35f);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(preAttackPos, punchPos, elapsed / duration);
            yield return null;
        }

        transform.position = punchPos;

        onHit?.Invoke();
        yield return new WaitForSeconds(0.05f);

        // возврат
        elapsed = 0f;
        while (elapsed < duration * 1.5f)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(punchPos, startPos, elapsed / (duration * 1.5f));
            yield return null;
        }

        transform.position = startPos;
    }

    private Vector3 GetDiagonalOffset(Vector3 attackerPos, Vector3 targetPos, bool facingRight)
    {
        var targetCell = GridManager.Instance.WorldToCell(targetPos);

        var leftCell = targetCell + Vector3Int.left;
        var rightCell = targetCell + Vector3Int.right;

        var leftFree = GridManager.Instance.IsCellWalkable(leftCell);
        var rightFree = GridManager.Instance.IsCellWalkable(rightCell);

        float sideX;

        if (leftFree && rightFree)
            sideX = facingRight ? 0.5f : -0.5f;
        else if (rightFree)
            sideX = 0.5f;
        else if (leftFree)
            sideX = -0.5f;
        else
            sideX = 0f;

        var verticalOffset = (targetPos.y - attackerPos.y) * 0.5f;
        return new Vector3(sideX, verticalOffset, 0);
    }

    private bool IsVerticalAttack(Vector3 from, Vector3 to)
    {
        var diff = to - from;
        return Mathf.Abs(diff.y) > Mathf.Abs(diff.x);
    }

    protected void UpdateSpriteFlip(float horizontalDirection)
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