using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Archer : BaseEnemy
{
    public override void Initialize()
    {
        MaxHealth = 25;
        AnimationSpeed = 4;
        Freeze = 0;
        Health = MaxHealth;
        MoveRange = 2;
    }
}
