using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : Entity
{
    public static Player Instance { get; private set; }

    public int Energy;
    public int MaxEnergy;

    public override void Initialize()
    {
        MaxHealth = 100;
        AnimationSpeed = 4;
        Freeze = 0;
        Health = MaxHealth;
        MaxEnergy = 15;
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

    public void SpendEnergy(int amount) => Instance.Energy = Mathf.Max(0, Instance.Energy - amount);
    /*public void ExecuteMove(Vector3Int targetCell)
    {
        MoveToCell(targetCell);
    }*/
}