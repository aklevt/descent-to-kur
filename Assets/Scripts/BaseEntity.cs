using System;
using System.Collections;
using Stats;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    [Header("Stats")] [SerializeField] private EntityStats baseStats;

    [Header("Stat Overrides")] [SerializeField]
    private int healthOverride;

    [SerializeField] private int moveRangeOverride;
    [SerializeField] private float moveSpeedOverride;
    [SerializeField] private int attackDamageOverride;

    [Header("Components")] [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField] protected Animator animator;

    public Vector3Int CurrentCell { get; private set; }
    public bool IsMoving { get; private set; }

    private Vector3 targetWorldPos;

    public int MaxHealth =>
        (baseStats != null) ? (healthOverride > 0 ? healthOverride : baseStats.maxHealth) : healthOverride;

    public int MoveRange => (baseStats != null)
        ? (moveRangeOverride > 0 ? moveRangeOverride : baseStats.moveRange)
        : moveRangeOverride;

    public float MoveSpeed => (baseStats != null)
        ? (moveSpeedOverride > 0 ? moveSpeedOverride : baseStats.moveSpeed)
        : moveSpeedOverride;

    public int AttackDamage => (baseStats != null)
        ? (attackDamageOverride > 0 ? attackDamageOverride : baseStats.baseAttackDamage)
        : attackDamageOverride;

    protected virtual void Start()
    {
        CurrentCell = GridManager.Instance.WorldToCell(transform.position);
        GridManager.Instance.RegisterFixedEntity(CurrentCell, gameObject);

        PlaceOnCell();
    }

    private void Update()
    {
        MoveSmoothly();
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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

        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.001f)
        {
            transform.position = targetWorldPos;
            IsMoving = false;

            var newCell = GridManager.Instance.WorldToCell(transform.position);
            SetLogicalPosition(newCell);

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

    protected virtual void OnArrivingToTarget()
    {
    }
}