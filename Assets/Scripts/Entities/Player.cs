using Abilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : Entity
{
    public static Player Instance { get; private set; }

    [NonSerialized]
    public int Energy;
    [NonSerialized]
    public int MaxEnergy;

    [Header("Abilities")] [SerializeField]
    private List<Ability> abilities = new();
    public IReadOnlyList<Ability> Abilities => abilities;

    public override void Initialize()
    {
        MaxHealth = 100;
        AnimationSpeed = 4;
        Freeze = 0;
        Health = MaxHealth;
        MaxEnergy = 20;
        Energy = MaxEnergy;
    }

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // грамотно переработать взаимодействие с публичными и приватными полями
    public void SpendEnergy(int amount) => Instance.Energy = Mathf.Max(0, Instance.Energy - amount);
    /*public void ExecuteMove(Vector3Int targetCell)
    {
        MoveToCell(targetCell);
    }*/
}