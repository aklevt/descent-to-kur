using Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// Базовый класс для всех персонажей (игрок, враги)
/// Наследуемые классы отвечают за все, что связано с сущностью: позиция на сетке, движение, анимация и список способностей
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [Header("Abilities")] [SerializeField] 
    private List<Ability> abilities = new();
    public IReadOnlyList<Ability> Abilities => abilities;

    //[Header("Stats")] [SerializeField] private EntityStats baseStats;
    //[Header("Live Stats")] [SerializeField]
    //private EntityRuntimeStats stats = new();
    //Header("Stats")] [SerializeField]
    [NonSerialized]
    public int MaxHealth;
    [NonSerialized]
    public float AnimationSpeed;
    [NonSerialized]
    public int Freeze;
    [NonSerialized]
    public int Health;

    //public EntityRuntimeStats Stats => stats;

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

    public virtual void Initialize() { }
    /*public void Initialize(EntityStats so)
    {
        if (so == null) return;
        MaxHealth = so.MaxHealth;
        Health = MaxHealth;
        MaxEnergy = so.MaxEnergy;
        Energy = MaxEnergy;
        AnimationSpeed = so.AnimationSpeed;
    }*/

    /*private void OnValidate()
    {

        if (!isCustomized) //&& baseStats != null)
        {
            Initialize();
            //stats.Initialize(baseStats);
        }
    }*/

    private void Update()
    {
        MoveSmoothly();
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Initialize();
        // Установка статов на свякий случай, пусть будет
        //if (!isCustomized) //(baseStats != null && !stats.isCustomized)
        //{
        //    Initialize();
        //    //stats.Initialize(baseStats);
        //}

        // Намеренно продублирована логика. Даже если в инспекторе изменить здоровье (isCustomized == true), оно сбросится после респауна
        //stats.Health = stats.MaxHealth;
        Health = MaxHealth;
    }

    private void PlaceOnCell()
    {
        UpdateTargetPosition(CurrentCell);
        transform.position = targetWorldPos;
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
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, AnimationSpeed * dt);

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

    /*public bool IsDead => Health <= 0;

    public bool HasEnergyForAction(int cost) => Energy >= cost;
    public void SpendEnergy(int amount) => Energy = Mathf.Max(0, Energy - amount);
    public void RestoreEnergy(int amount) => Energy = Mathf.Min(MaxEnergy, Energy + amount);

    public void ApplyDamage(int amount)
    {
        Health = Mathf.Max(0, Health - amount);
    }

    public void ApplyHeal(int amount)
    {
        Health = Mathf.Min(MaxHealth, Health + amount);
    }*/
}